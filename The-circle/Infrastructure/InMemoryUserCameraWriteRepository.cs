using System.Collections.Concurrent;
using The_circle.Application;
using The_circle.Domain;
using The_circle.Domain.Models;

namespace The_circle.Infrastructure;

public class InMemoryUserCameraWriteRepository : IUserCameraWriteRepository
{
    private readonly List<VideoChunk> _chunks = new();

    public Task SaveChunkAsync(VideoChunk videoChunk)
    {
        _chunks.Add(videoChunk);
        return Task.CompletedTask;
    }
}
