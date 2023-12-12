namespace Domain.Dtos.Token
{
    public class JWTResult
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string JWToken { get; set; }
        public DateTime JWTExpires { get; set; }
        public DateTime SessionDuration { get; set; }
    }
}
