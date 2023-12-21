using Application.Features.Auth.Login;
using Application.Features.Auth.RefreshSession;
using Application.Features.Auth.Register;
using Application.Interfaces.AppServices.ConnectionServices;
using Asp.Versioning;
using Domain.Dtos.Auth;
using Domain.Dtos.Token;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebBlog.Controllers.V1
{
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        private readonly IIpManagerService _ipManager;

        public AuthController(IIpManagerService ipManager)
        {
            _ipManager = ipManager;
        }

        [HttpPost("register")]
        [SwaggerResponse(StatusCodes.Status200OK, "Register succesful")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid inputs", typeof(ValidationProblemDetails))]

        public async Task<IActionResult> AuthRegister([FromBody] RegisterCommand request, CancellationToken cancellationToken)
        {
            var ipAddress = _ipManager.GenerateIpAddress();

            var result = await Mediator.Send(new RegisterCommand
            {
                Username = request.Username,
                Email = request.Email,
                Birthday = request.Birthday,
                Password = request.Password,
                RepeatPassword = request.RepeatPassword,
                IpAddress = ipAddress,
            },cancellationToken);

            return result.ToOk();
        }
        [HttpPost("signin")]
        [SwaggerResponse(StatusCodes.Status200OK, "Login succesful")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid inputs", typeof(ValidationProblemDetails))]

        public async Task<IActionResult> AuthLogin([FromBody] LoginDto request, CancellationToken cancellationToken)
        {
            var ipAddress = _ipManager.GenerateIpAddress();
                
            var result = await Mediator.Send(new LoginCommand
            {
                Email = request.Email,
                Password = request.Password,
                Ip = ipAddress,
            },cancellationToken);

            return result.ToOk();
        }

        [HttpPost("refresh-token")]
        [SwaggerResponse(StatusCodes.Status200OK, "Login succesful")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid inputs", typeof(ValidationProblemDetails))]
        public async Task<IActionResult> RefreshSessionToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var ipAddress = _ipManager.GenerateIpAddress();
            var refreshToken = HttpContext.Request.Cookies["refreshToken"];

            var result = await Mediator.Send(new RefreshSessionCommand
            {
                UserId = request.UserId,
                IpAddress = ipAddress,
                RefreshToken = refreshToken,
            }, cancellationToken);

            return result.ToOk();
        }
    }
}
