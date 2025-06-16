using The_circle.Domain;

namespace The_circle.Application;

public interface IUserCameraWriteRepository
{
    Task AddAsync(UserCamera userCamera);
    Task SaveAsync(UserCamera userCamera);
}