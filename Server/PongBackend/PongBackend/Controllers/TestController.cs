using Microsoft.AspNetCore.Mvc;
using PongBackend.DTOs;

namespace PongBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                message = "Backend is running"
            });
        }

        [HttpPost]
        public IActionResult Post([FromBody] TestMessageRequest request)
        {
            return Ok(new
            {
                message = request.Message
            });
        }
    }
}
