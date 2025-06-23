using The_circle.Domain.Models;

namespace The_circle.Domain;

public class VideoChunk
{
    public Guid     Id           { get; set; } = Guid.NewGuid();
    public Guid     StreamId     { get; set; }
    public int      ChunkIndex   { get; set; }
    public byte[]   ChunkData    { get; set; } = [];
    public byte[]   Signature    { get; set; } = [];
    public byte[]   Certificate  { get; set; } = [];
    public DateTime Timestamp    { get; set; } = DateTime.UtcNow;
}
