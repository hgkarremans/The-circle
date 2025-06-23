using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using The_circle.Domain;

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

            _buffer.SetFrame(streamId, chunkIndex, chunk, signature, certBytes);
        }
    }
}
