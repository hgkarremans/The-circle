using The_circle.Domain;
using The_circle.Domain.Models;

namespace The_circle.Application;

public interface IUserCameraWriteRepository
{
    Task SaveChunkAsync(VideoChunk videoChunk);
}