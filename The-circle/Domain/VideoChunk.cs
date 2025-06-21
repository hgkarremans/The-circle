using The_circle.Domain.Models;

namespace The_circle.Domain;

public class VideoChunk : IVideoChunk
{
    public Guid StreamId { get; set; }
    public int ChunkIndex { get; set; }
    public byte[] ChunkData { get; set; }
    public DateTime Timestamp { get; set; }
}
