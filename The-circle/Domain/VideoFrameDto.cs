namespace The_circle.Domain;

public class VideoFrameDto
{
    public Guid StreamId { get; set; }
    public int ChunkIndex { get; set; }
    public byte[] Chunk { get; set; }
    public byte[] Signature { get; set; }
    public byte[] Certificate { get; set; }
}
