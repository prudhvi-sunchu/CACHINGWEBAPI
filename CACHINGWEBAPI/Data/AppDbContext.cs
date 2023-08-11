using CACHINGWEBAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CACHINGWEBAPI.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Driver> Drivers { get; set; } //adding driver
        public AppDbContext(DbContextOptions<AppDbContext> options):
            base(options)
        {
            
        }
    }
}
