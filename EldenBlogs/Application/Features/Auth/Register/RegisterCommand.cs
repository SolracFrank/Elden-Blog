using LanguageExt.Common;
using MediatR;

namespace Application.Features.Auth.Register
{
    public class RegisterCommand : IRequest<Result<string>>
    {
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string RepeatPassword { get; set; }
        public required DateTime Birthday { get; set; } = DateTime.UtcNow;
        public string IpAddress { get; set; }
    }
}
