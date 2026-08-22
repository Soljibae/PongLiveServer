using Microsoft.EntityFrameworkCore;
using PongBackend.Models;

namespace PongBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
    }
}