namespace PongBackend.DTOs.Auth
{
    public class SignUpRequest
    {
        public string Id { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
    }
}
