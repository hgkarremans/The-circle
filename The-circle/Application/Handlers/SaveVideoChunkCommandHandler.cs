using MediatR;
using The_circle.Application.Commands;
using The_circle.Domain.Models;

namespace The_circle.Application.Handlers;

public class SaveVideoChunkCommandHandler : IRequestHandler<SaveVideoChunkCommand, Unit>
{
    private readonly IUserCameraWriteRepository _repository;

    public SaveVideoChunkCommandHandler(IUserCameraWriteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(SaveVideoChunkCommand request, CancellationToken cancellationToken)
    {
        var entity = new VideoChunk
        {
            
            StreamId = Guid.Parse(request.StreamId),
            ChunkIndex = request.ChunkIndex,
            ChunkData = request.Chunk,
            Timestamp = DateTime.UtcNow
        };

        await _repository.SaveChunkAsync(entity);
        return Unit.Value;
    }
}
