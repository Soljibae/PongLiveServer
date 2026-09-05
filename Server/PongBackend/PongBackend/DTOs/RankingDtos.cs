namespace PongBackend.DTOs
{
    public class RankingUserResponse
    {
        public int Rank { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int RankingScore { get; set; }
    }

    public class RankingResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public List<RankingUserResponse> Rankings { get; set; }
            = new();

        public RankingUserResponse? MyRanking { get; set; }
    }
}
