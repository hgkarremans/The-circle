namespace The_circle.Application.Services;

public class VideoFrameBufferService
{
    private readonly object _lock = new();
    private readonly Dictionary<Guid, (int Index, string Base64, DateTime LastUpdated)> _streams = new();

    public void SetFrame(Guid streamId, int chunkIndex, byte[] frame)
    {
        lock (_lock)
        {
            if (_streams.TryGetValue(streamId, out var existing) && chunkIndex <= existing.Index)
                return;

            var base64 = Convert.ToBase64String(frame);
            _streams[streamId] = (chunkIndex, base64, DateTime.UtcNow);
        }
    }

    public string? GetFrame(Guid streamId)
    {
        lock (_lock)
        {
            return _streams.TryGetValue(streamId, out var data) ? data.Base64 : null;
        }
    }

    public List<Guid> GetActiveStreamIds(TimeSpan maxAge)
    {
        lock (_lock)
        {
            var now = DateTime.UtcNow;
            return _streams
                .Where(pair => now - pair.Value.LastUpdated < maxAge)
                .Select(pair => pair.Key)
                .ToList();
        }
    }
}


