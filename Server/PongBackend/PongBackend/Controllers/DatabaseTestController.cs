using Microsoft.AspNetCore.Mvc;
using PongBackend.Data;

namespace PongBackend.Controllers
{
    [ApiController]
    [Route("api/database")]
    public class DatabaseTestController : ControllerBase
    {
        private readonly PongDbContext dbContext;

        public DatabaseTestController(PongDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            bool canConnect = await dbContext.Database.CanConnectAsync();

            if (!canConnect)
            {
                return StatusCode( 500, new { message = "Database connection failed" });
            }

            return Ok(new
            {
                message = "PostgreSQL connection success"
            });
        }
    }
}
