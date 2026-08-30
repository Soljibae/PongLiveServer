using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using PongBackend.Data;
using PongBackend.DTOs.Auth;
using PongBackend.Models;
using PongBackend.Rules;
using PongBackend.Services;

namespace PongBackend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        private readonly PasswordHasher<User> _passwordHasher;

        private readonly JwtTokenService _jwtTokenService;

        public AuthController(
            AppDbContext dbContext,
            PasswordHasher<User> passwordHasher,
            JwtTokenService jwtTokenService)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
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

        [EnableRateLimiting("AccountSignUp")]
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(
            [FromBody] SignUpRequest request)
        {
            bool isIdValid = IsAlphaNumeric(
                request.Id,
                AccountRules.IdMinLength,
                AccountRules.IdMaxLength
            );

            bool isPasswordValid = IsAlphaNumeric(
                request.Password,
                AccountRules.PasswordMinLength,
                AccountRules.PasswordMaxLength
            );

            bool isNicknameValid = IsAlphaNumeric(
                request.Nickname,
                AccountRules.NicknameMinLength,
                AccountRules.NicknameMaxLength
            );

            if (!isIdValid)
            {
                return BadRequest(new SignUpResponse
                {
                    Success = false,
                    Message = "Invalid ID format."
                });
            }

            if (!isPasswordValid)
            {
                return BadRequest(new SignUpResponse
                {
                    Success = false,
                    Message = "Invalid password format."
                });
            }

            if (!isNicknameValid)
            {
                return BadRequest(new SignUpResponse
                {
                    Success = false,
                    Message = "Invalid nickname format."
                });
            }

            bool idExists = await _dbContext.Users
                .AnyAsync(user => user.LoginId == request.Id);

            if (idExists)
            {
                return Conflict(new SignUpResponse
                {
                    Success = false,
                    Message = "ID is already in use."
                });
            }

            bool nicknameExists = await _dbContext.Users
                .AnyAsync(user => user.Nickname == request.Nickname);

            if (nicknameExists)
            {
                return Conflict(new SignUpResponse
                {
                    Success = false,
                    Message = "Nickname is already in use."
                });
            }

            User user = new User
            {
                LoginId = request.Id,
                Nickname = request.Nickname,
                CreatedAt = DateTime.UtcNow,
                Wins = 0,
                Losses = 0
            };

            user.PasswordHash =
                _passwordHasher.HashPassword(
                    user,
                    request.Password
                );

            try
            {
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new SignUpResponse
                    {
                        Success = false,
                        Message = "Failed to create account."
                    }
                );
            }

            return Ok(new SignUpResponse
            {
                Success = true,
                Message = "Sign up successful."
            });
        }

        [EnableRateLimiting("AccountLogin")]
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request)
        {
            bool isIdValid = IsAlphaNumeric(
                request.Id,
                AccountRules.IdMinLength,
                AccountRules.IdMaxLength
            );

            bool isPasswordValid = IsAlphaNumeric(
                request.Password,
                AccountRules.PasswordMinLength,
                AccountRules.PasswordMaxLength
            );

            if (!isIdValid || !isPasswordValid)
            {
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = "Invalid ID or password."
                });
            }

            User? user = await _dbContext.Users
                .FirstOrDefaultAsync(
                    user => user.LoginId == request.Id
                );

            if (user == null)
            {
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = "Invalid ID or password."
                });
            }

            PasswordVerificationResult result =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    request.Password
                );

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = "Invalid ID or password."
                });
            }
            string token = _jwtTokenService.CreateToken(user.UserId.ToString(), "user");

            return Ok(new LoginResponse
            {
                Success = true,
                Message = "Login successful.",
                Token = token,
                Nickname = user.Nickname
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