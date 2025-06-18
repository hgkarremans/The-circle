using System.Net;
using System.Net.Sockets;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using The_circle.Application.Commands;

namespace The_circle.Controllers;

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

        var payload = BuildPayload(streamId, chunkIndex, chunk);
        await _udpClient.SendAsync(payload, payload.Length, _broadcastEndpoint);

        await _mediator.Send(new SaveVideoChunkCommand(streamId, chunkIndex, chunk));

        return Ok();
    }

    private byte[] BuildPayload(string streamId, int chunkIndex, byte[] chunk)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);
        writer.Write(Guid.Parse(streamId).ToByteArray());
        writer.Write(chunkIndex);
        writer.Write(chunk.Length);
        writer.Write(chunk);
        return ms.ToArray();
    }
}
