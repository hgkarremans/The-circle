using System.Collections.Concurrent;
using The_circle.Domain;

namespace The_circle.Application.Services;

public class VideoFrameBufferService
{
    private readonly ConcurrentDictionary<Guid, (VideoFrameDto Frame, DateTime Timestamp)> _frames = new();

    public void SetFrame(Guid streamId, int chunkIndex, byte[] chunk, byte[] signature, byte[] certificate)
    {
        var dto = new VideoFrameDto
        {
            StreamId = streamId,
            ChunkIndex = chunkIndex,
            Chunk = chunk,
            Signature = signature,
            Certificate = certificate
        };

        _frames[streamId] = (dto, DateTime.UtcNow);
    }

    public VideoFrameDto? GetFrameWithMetadata(Guid streamId)
    {
        return _frames.TryGetValue(streamId, out var result) ? result.Frame : null;
    }

    public List<Guid> GetActiveStreamIds(TimeSpan recentThreshold)
    {
        var cutoff = DateTime.UtcNow - recentThreshold;

        return _frames
            .Where(kvp => kvp.Value.Timestamp > cutoff)
            .Select(kvp => kvp.Key)
            .ToList();
    }
}