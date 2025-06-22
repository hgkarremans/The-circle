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
    private readonly IWebHostEnvironment _env;

    public VideoChunkController(IMediator mediator, IWebHostEnvironment env)
    {
        _mediator = mediator;
        _env = env;
        _udpClient = new UdpClient();
        _broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, 9000);
    }

    [HttpPost]
    public async Task<IActionResult> ReceiveChunk()
    {
        // 0. Haal certificaat op uit sessie
        var certRaw = HttpContext.Session.Get("TruYouCert");
        if (certRaw == null)
            return Unauthorized("Je bent niet ingelogd of certificaat ontbreekt.");

        var cert = new X509Certificate2(certRaw);
        using var rsa = cert.GetRSAPrivateKey();

        // 1. Haal headers op
        var streamId = Request.Headers["X-Stream-Id"].ToString();
        var chunkIndex = int.Parse(Request.Headers["X-Chunk-Index"]);

        // 2. Lees de chunk (JPEG frame)
        using var ms = new MemoryStream();
        await Request.Body.CopyToAsync(ms);
        var chunk = ms.ToArray();

        // 3. Genereer handtekening over de chunk
        var signature = rsa.SignData(chunk, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        // 4. Exporteer .crt (zodat ontvanger kan valideren)
        var certBytes = cert.Export(X509ContentType.Cert);

        // 5. Bouw UDP payload
        var payload = BuildPayload(streamId, chunkIndex, chunk, signature, certBytes);
        await _udpClient.SendAsync(payload, payload.Length, _broadcastEndpoint);

        // 6. Opslaan via CQRS
        await _mediator.Send(new SaveVideoChunkCommand(streamId, chunkIndex, chunk));
        return Ok();
    }


    private byte[] BuildPayload(string streamId, int chunkIndex, byte[] chunk, byte[] signature, byte[] certBytes)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        writer.Write(Guid.Parse(streamId).ToByteArray()); // 16 bytes
        writer.Write(chunkIndex);                         // 4 bytes
        writer.Write(chunk.Length);                       // 4 bytes
        writer.Write(chunk);                              // n bytes

        writer.Write((ushort)signature.Length);           // 2 bytes
        writer.Write(signature);                          // m bytes

        writer.Write((ushort)certBytes.Length);           // 2 bytes
        writer.Write(certBytes);                          // k bytes

        return ms.ToArray();
    }
}
