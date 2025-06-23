using Microsoft.EntityFrameworkCore;
using The_circle.Domain;

namespace The_circle.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<VideoChunk> VideoChunks { get; set; }
}