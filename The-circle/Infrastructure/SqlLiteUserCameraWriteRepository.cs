using The_circle.Application;
using The_circle.Domain;

namespace The_circle.Infrastructure;

public class SqlLiteUserCameraWriteRepository : IUserCameraWriteRepository
{
    private readonly ApplicationDbContext _context;

    public SqlLiteUserCameraWriteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SaveChunkAsync(VideoChunk chunk)
    {
        _context.VideoChunks.Add(chunk);
        await _context.SaveChangesAsync();
    }
}