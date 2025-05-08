using Microsoft.EntityFrameworkCore;

namespace VideoGameApi.Data
{
    public class VideoGameDbContext(DbContextOptions<VideoGameDbContext> options) : DbContext(options)
    {
        // works:
        // public DbSet<VideoGame> VideoGames { get; set; }
        
        // best practice:
        public DbSet<VideoGame> VideoGames => Set<VideoGame>();
    }
}
