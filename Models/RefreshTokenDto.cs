namespace WebApplication6.Models
{
    public class RefreshTokenDto
    {
        public required Guid UserId { get; set; }
        public required string RefreshToken { get; set; }
    }
}
