using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace The_circle.Application.Services;

public class UdpVideoListenerService : BackgroundService
{
    private readonly VideoFrameBufferService _buffer;

    public UdpVideoListenerService(VideoFrameBufferService buffer)
    {
        _buffer = buffer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var udpClient = new UdpClient(9000);
        udpClient.EnableBroadcast = true;

        var rootCert = new X509Certificate2("../truYou-ca/circle-root.crt");

        while (!stoppingToken.IsCancellationRequested)
        {
            var result = await udpClient.ReceiveAsync();
            using var ms = new MemoryStream(result.Buffer);
            using var reader = new BinaryReader(ms);

            var streamId = new Guid(reader.ReadBytes(16));
            var chunkIndex = reader.ReadInt32();
            var chunkLength = reader.ReadInt32();
            var chunk = reader.ReadBytes(chunkLength);

            var sigLen = reader.ReadUInt16();
            var signature = reader.ReadBytes(sigLen);

            var certLen = reader.ReadUInt16();
            var certBytes = reader.ReadBytes(certLen);
            var senderCert = new X509Certificate2(certBytes);

            var chain = new X509Chain();
            chain.ChainPolicy.ExtraStore.Add(rootCert);
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

            bool certValid = chain.Build(senderCert);

            var rsa = senderCert.GetRSAPublicKey();
            bool sigValid = rsa.VerifyData(chunk, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            if (certValid && sigValid)
            {
                _buffer.SetFrame(streamId, chunkIndex, chunk);
                Console.WriteLine($"Verified frame {chunkIndex} from {senderCert.Subject}");
            }
            else
            {
                Console.WriteLine($"Rejected frame {chunkIndex}: Cert valid? {certValid}, Signature valid? {sigValid}");
            }
        }
    }

}
