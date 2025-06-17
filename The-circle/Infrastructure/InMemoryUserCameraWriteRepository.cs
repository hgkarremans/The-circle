using System.Collections.Concurrent;
using The_circle.Application;
using The_circle.Domain;
using The_circle.Domain.Models;

namespace The_circle.Infrastructure;

public class InMemoryUserCameraWriteRepository : IUserCameraWriteRepository
{
    private static readonly ConcurrentDictionary<Guid, List<VideoChunk>> Storage = new();

    public Task SaveChunkAsync(VideoChunk videoChunk)
    {
        var chunks = Storage.GetOrAdd(videoChunk.StreamId, _ => new List<VideoChunk>());

        lock (chunks) 
        {
            chunks.Add(videoChunk);
        }

        return Task.CompletedTask;
    }
}