using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using The_circle.Application.Commands;

namespace The_circle.Presentation.Controllers;

[ApiController]
[Route("api/videochunk")]
public class VideoChunkController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly UdpClient _udpClient;
    private readonly IPEndPoint _broadcastEndpoint;

    public VideoChunkController(IMediator mediator)
    {
        _mediator = mediator;
        _udpClient = new UdpClient { EnableBroadcast = true };
        _broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, 9000);
    }

    [HttpPost]
    public async Task<IActionResult> ReceiveChunk()
    {
        await using var ms = new MemoryStream();
        await Request.Body.CopyToAsync(ms);
        var body = ms.ToArray();

        try
        {
            using var reader = new BinaryReader(new MemoryStream(body));

            var streamId = new Guid(reader.ReadBytes(16));
            var chunkIndex = reader.ReadInt32();
            var chunkLength = reader.ReadInt32();

            if (chunkLength < 0 || chunkLength > 1_000_000)
                return BadRequest("Invalid chunk length.");

            var chunk = reader.ReadBytes(chunkLength);

            if (reader.BaseStream.Position + 2 > reader.BaseStream.Length)
                return BadRequest("Missing signature length.");

            var sigLength = reader.ReadUInt16();

            if (reader.BaseStream.Position + sigLength + 2 > reader.BaseStream.Length)
                return BadRequest("Incomplete signature or missing certificate length.");

            var signature = reader.ReadBytes(sigLength);

            var certLength = reader.ReadUInt16();

            if (reader.BaseStream.Position + certLength > reader.BaseStream.Length)
                return BadRequest("Incomplete certificate.");

            var certBytes = reader.ReadBytes(certLength);
            var cert = new X509Certificate2(certBytes);

            // Broadcast via UDP
            var payload = BuildPayload(streamId, chunkIndex, chunk, signature, certBytes);
            await _udpClient.SendAsync(payload, payload.Length, _broadcastEndpoint);

            // Opslaan via CQRS
            await _mediator.Send(new SaveVideoChunkCommand(streamId.ToString(), chunkIndex, chunk));

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[VideoChunkController] Error: {ex.Message}");
            return StatusCode(500, "Fout bij verwerken videochunk.");
        }
    }

    private static byte[] BuildPayload(
        Guid streamId,
        int chunkIndex,
        byte[] chunk,
        byte[] signature,
        byte[] certBytes)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        writer.Write(streamId.ToByteArray());       // 16 bytes
        writer.Write(chunkIndex);                   // 4 bytes
        writer.Write(chunk.Length);                 // 4 bytes
        writer.Write(chunk);                        // n bytes

        writer.Write((ushort)signature.Length);     // 2 bytes
        writer.Write(signature);                    // m bytes

        writer.Write((ushort)certBytes.Length);     // 2 bytes
        writer.Write(certBytes);                    // k bytes

        return ms.ToArray();
    }
}
