namespace The_circle.Domain.Models;

public class IVideoChunk 
{
    Guid StreamId { get; set; }
    int ChunkIndex { get; set; }
    byte[] ChunkData { get; set; }
    DateTime Timestamp { get; set; }
}