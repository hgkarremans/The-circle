using The_circle.Application;
using The_circle.Domain;

namespace The_circle.Infrastructure;

public class InMemoryUserCameraWriteRepository : IUserCameraWriteRepository
{
    public Task AddAsync(UserCamera userCamera)
    {
        UserCameraStore.Cameras.TryAdd(userCamera.UserId, userCamera);
        return Task.CompletedTask;
    }

    public Task SaveAsync(UserCamera userCamera)
    {
        UserCameraStore.Cameras[userCamera.UserId] = userCamera;
        return Task.CompletedTask;
    }
}