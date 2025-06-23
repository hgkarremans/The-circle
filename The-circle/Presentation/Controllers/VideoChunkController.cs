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
            var streamId    = new Guid(reader.ReadBytes(16));
            var chunkIndex  = reader.ReadInt32();
            var chunkLength = reader.ReadInt32();

            // validation omitted for brevity...
            var chunk = reader.ReadBytes(chunkLength);
            var sigLength  = reader.ReadUInt16();
            var signature  = reader.ReadBytes(sigLength);
            var certLength = reader.ReadUInt16();
            var certBytes  = reader.ReadBytes(certLength);

            // Broadcast via UDP
            var payload = BuildPayload(streamId, chunkIndex, chunk, signature, certBytes);
            await _udpClient.SendAsync(payload, payload.Length, _broadcastEndpoint);

            // Persist with signature & certificate
            var cmd = new SaveVideoChunkCommand(
                streamId.ToString(),
                chunkIndex,
                chunk,
                signature,
                certBytes
            );
            await _mediator.Send(cmd);

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
        writer.Write(streamId.ToByteArray());
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
