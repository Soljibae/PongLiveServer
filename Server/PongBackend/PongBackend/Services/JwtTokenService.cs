using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PongBackend.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateToken(
           string subjectId,
           string subjectType)
        {
            string key =
                _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "JWT Key was not found."
                );

            string issuer =
                _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException(
                    "JWT Issuer was not found."
                );

            string audience =
                _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException(
                    "JWT Audience was not found."
                );

            Claim[] claims =
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    subjectId
                ),

                new Claim(
                    "type",
                    subjectType
                )
            };

            SymmetricSecurityKey securityKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(key)
                );

            SigningCredentials credentials =
                new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256
                );

            JwtSecurityToken token =
                new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}
