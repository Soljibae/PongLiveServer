using System.ComponentModel.DataAnnotations.Schema;

namespace PongBackend.Models
{
    [Table("users")]
    public class User
    {
        [Column("user_id")]
        public long UserId { get; set; }

        [Column("login_id")]
        public string LoginId { get; set; } = string.Empty;

        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("nickname")]
        public string? Nickname { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("wins")]
        public int Wins { get; set; }

        [Column("losses")]
        public int Losses { get; set; }

        [Column("ranking_score")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int RankingScore { get; private set; }
    }
}
