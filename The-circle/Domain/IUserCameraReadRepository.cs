using The_circle.Domain;

namespace The_circle.Application;

public interface IUserCameraReadRepository
{
    Task<UserCamera?> GetByUserIdAsync(Guid userId);
}