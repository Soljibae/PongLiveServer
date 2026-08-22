using PongBackend.Data;
using PongBackend.DTOs.Auth;
using PongBackend.Rules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace PongBackend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public AuthController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [EnableRateLimiting("AccountCheck")]
        [HttpGet("check-id")]
        public async Task<IActionResult> CheckId([FromQuery] string id)
        {
            bool isValid = IsAlphaNumeric(
                id,
                AccountRules.IdMinLength,
                AccountRules.IdMaxLength
            );

            if (!isValid)
            {
                return BadRequest(new AvailabilityResponse
                {
                    Available = false,
                    Message = "Invalid ID format."
                });
            }

            bool exists = await _dbContext.Users
                .AnyAsync(user => user.LoginId == id);

            if (exists)
            {
                return Ok(new AvailabilityResponse
                {
                    Available = false,
                    Message = "ID is already in use."
                });
            }

            return Ok(new AvailabilityResponse
            {
                Available = true,
                Message = "ID is available."
            });
        }

        [EnableRateLimiting("AccountCheck")]
        [HttpGet("check-nickname")]
        public async Task<IActionResult> CheckNickname(
            [FromQuery] string nickname)
        {
            bool isValid = IsAlphaNumeric(
                nickname,
                AccountRules.NicknameMinLength,
                AccountRules.NicknameMaxLength
            );

            if (!isValid)
            {
                return BadRequest(new AvailabilityResponse
                {
                    Available = false,
                    Message = "Invalid nickname format."
                });
            }

            bool exists = await _dbContext.Users
                .AnyAsync(user => user.Nickname == nickname);

            if (exists)
            {
                return Ok(new AvailabilityResponse
                {
                    Available = false,
                    Message = "Nickname is already in use."
                });
            }

            return Ok(new AvailabilityResponse
            {
                Available = true,
                Message = "Nickname is available."
            });
        }

        private static bool IsAlphaNumeric(
            string value,
            int minLength,
            int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            if (value.Length < minLength ||
                value.Length > maxLength)
            {
                return false;
            }

            foreach (char c in value)
            {
                bool isUpper = c >= 'A' && c <= 'Z';
                bool isLower = c >= 'a' && c <= 'z';
                bool isNumber = c >= '0' && c <= '9';

                if (!isUpper && !isLower && !isNumber)
                    return false;
            }

            return true;
        }
    }
}