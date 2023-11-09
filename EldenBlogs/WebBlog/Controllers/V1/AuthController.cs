using Application.Features.Auth.Login;
using Application.Features.Auth.Register;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebBlog.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        [HttpPost("register")]
        [SwaggerResponse(StatusCodes.Status200OK, "Register succesful")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid inputs", typeof(ValidationProblemDetails))]

        public async Task<IActionResult> AuthRegister([FromBody] RegisterCommand request, CancellationToken cancellationToken)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

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

        public async Task<IActionResult> AuthLogin([FromBody] LoginCommand request, CancellationToken cancellationToken)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var result = await Mediator.Send(new LoginCommand
            {
                Email = request.Email,
                Password = request.Password,
                Ip = ipAddress,
            },cancellationToken);

            return result.ToOk();
        }

    }
}
