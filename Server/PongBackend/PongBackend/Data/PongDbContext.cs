using Microsoft.EntityFrameworkCore;

namespace PongBackend.Data
{
    public class PongDbContext : DbContext
    {
        public PongDbContext(DbContextOptions<PongDbContext> options) : base(options) {}
    }
}
