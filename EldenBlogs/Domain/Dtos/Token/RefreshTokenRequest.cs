namespace Domain.Dtos.Token
{
    public class RefreshTokenRequest
    {
        public required string UserId { get; set; }
        public required string IpAddress { get; set; }
    }
}
