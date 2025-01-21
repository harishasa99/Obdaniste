using ActivityService.Models;
using Microsoft.EntityFrameworkCore;

namespace ActivityService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Activity> Activities { get; set; }
        public DbSet<Event> Events { get; set; }
    }
}
