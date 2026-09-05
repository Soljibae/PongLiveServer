using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PongBackend.Data;
using PongBackend.DTOs;
using PongBackend.Models;
using System.Security.Claims;

namespace PongBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RankingController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public RankingController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<RankingResponse>> GetRanking()
        {
            string? userIdValue =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdValue, out int userId))
            {
                return Unauthorized(new RankingResponse
                {
                    Success = false,
                    Message = "Invalid token."
                });
            }

            var currentUser = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(user => user.UserId == userId);

            if (currentUser == null)
            {
                return NotFound(new RankingResponse
                {
                    Success = false,
                    Message = "User not found."
                });
            }

            var topUsers = await _dbContext.Users
                .AsNoTracking()
                .OrderByDescending(user => user.RankingScore)
                .ThenBy(user => user.UserId)
                .Take(10)
                .ToListAsync();

            List<RankingUserResponse> rankings = new();

            int currentRank = 0;
            int? previousScore = null;

            for (int i = 0; i < topUsers.Count; i++)
            {
                var user = topUsers[i];

                if (previousScore == null ||
                    user.RankingScore != previousScore)
                {
                    currentRank = i + 1;
                }

                rankings.Add(new RankingUserResponse
                {
                    Rank = currentRank,
                    Nickname = user.Nickname,
                    Wins = user.Wins,
                    Losses = user.Losses,
                    RankingScore = user.RankingScore
                });

                previousScore = user.RankingScore;
            }

            int myRank =
                await _dbContext.Users.CountAsync(user =>
                    user.RankingScore > currentUser.RankingScore
                ) + 1;

            return Ok(new RankingResponse
            {
                Success = true,
                Message = "Ranking data retrieved successfully.",
                Rankings = rankings,
                MyRanking = new RankingUserResponse
                {
                    Rank = myRank,
                    Nickname = currentUser.Nickname,
                    Wins = currentUser.Wins,
                    Losses = currentUser.Losses,
                    RankingScore = currentUser.RankingScore
                }
            });
        }
    }
}
