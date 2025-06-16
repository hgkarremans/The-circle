using System.Collections.Concurrent;
using The_circle.Domain;

namespace The_circle.Infrastructure;

public class UserCameraStore
{
    public static ConcurrentDictionary<Guid, UserCamera> Cameras { get; } = new();
}