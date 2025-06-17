using MediatR;

namespace The_circle.Application.Commands;

public class SaveVideoChunkCommand : IRequest<Unit>
{
    public String StreamId { get; set; }
    public int ChunkIndex { get; set; }
    public byte[] Chunk { get; set; }

    public SaveVideoChunkCommand(String streamId, int chunkIndex, byte[] chunk)
    {
        StreamId = streamId;
        ChunkIndex = chunkIndex;
        Chunk = chunk;
    }
}