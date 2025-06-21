using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using The_circle.Application.Commands;

namespace The_circle.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VideoChunkController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly UdpClient _udpClient;
    private readonly IPEndPoint _broadcastEndpoint;

    public VideoChunkController(IMediator mediator)
    {
        _mediator = mediator;
        _udpClient = new UdpClient();
        _broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, 9000); 
    }

    [HttpPost]
    public async Task<IActionResult> ReceiveChunk()
    {
        var streamId = Request.Headers["X-Stream-Id"].ToString();
        var chunkIndex = int.Parse(Request.Headers["X-Chunk-Index"]);

        using var ms = new MemoryStream();
        await Request.Body.CopyToAsync(ms);
        var chunk = ms.ToArray();

        // 1. Laad certificaat
        var cert = new X509Certificate2("../truYou-ca/hg.karremans/karremans.pfx", "test123");
        using var rsa = cert.GetRSAPrivateKey();

        // 2. Maak handtekening
        var signature = rsa.SignData(chunk, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var certBytes = cert.Export(X509ContentType.Cert); // .crt-formaat

        // 3. Bouw pakket met alles erin
        var payload = BuildPayload(streamId, chunkIndex, chunk, signature, certBytes);
        await _udpClient.SendAsync(payload, payload.Length, _broadcastEndpoint);

        // 4. Opslaan zoals eerder
        await _mediator.Send(new SaveVideoChunkCommand(streamId, chunkIndex, chunk));
        return Ok();
    }

    private byte[] BuildPayload(string streamId, int chunkIndex, byte[] chunk, byte[] signature, byte[] certBytes)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        writer.Write(Guid.Parse(streamId).ToByteArray());
        writer.Write(chunkIndex);
        writer.Write(chunk.Length);
        writer.Write(chunk);

        writer.Write((ushort)signature.Length);
        writer.Write(signature);

        writer.Write((ushort)certBytes.Length);
        writer.Write(certBytes);

        return ms.ToArray();
    }

}
