using The_circle.Application;
using The_circle.Domain;

namespace The_circle.Infrastructure;

public class InMemoryUserCameraReadRepository : IUserCameraReadRepository
{
    public Task<UserCamera?> GetByUserIdAsync(Guid userId)
    {
        UserCamera? userCamera = null;
        UserCameraStore.Cameras.TryGetValue(userId, out userCamera);
        return Task.FromResult(userCamera);
    }
}