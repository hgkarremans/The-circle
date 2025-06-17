using MediatR;
using Microsoft.AspNetCore.SignalR;
using The_circle.Application.Commands;

namespace The_circle.Presentation.Hubs;

public class CameraHub : Hub
{
    private readonly IMediator _mediator;

    public CameraHub(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task SendVideoChunk(byte[] chunk, string streamId, int chunkIndex)
    {
        await Clients.Others.SendAsync("ReceiveVideoChunk", chunk, streamId, chunkIndex);

        await _mediator.Send(new SaveVideoChunkCommand(streamId, chunkIndex, chunk));
    }
}