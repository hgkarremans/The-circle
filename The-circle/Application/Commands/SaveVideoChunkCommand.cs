using MediatR;

namespace The_circle.Application.Commands;

public class SaveVideoChunkCommand : IRequest<Unit>
{
    public string StreamId       { get; }
    public int    ChunkIndex     { get; }
    public byte[] Chunk          { get; }
    public byte[] Signature      { get; }
    public byte[] Certificate    { get; }

    public SaveVideoChunkCommand(string streamId, int chunkIndex, byte[] chunk, byte[] signature, byte[] certificate)
    {
        StreamId    = streamId;
        ChunkIndex  = chunkIndex;
        Chunk       = chunk;
        Signature   = signature;
        Certificate = certificate;
    }
}

