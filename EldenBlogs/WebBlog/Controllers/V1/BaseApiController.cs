using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebBlog.Controllers.V1
{
    [ApiVersion(1.0)]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Test", typeof(string))]
        public async Task<IActionResult> BaseEndpoint()
        {
            return Ok("HI, I'm an Endpoint");
        }
        [HttpGet("otro")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> BaseEndpoint2()
        {
            return Ok("HI, I'm an Endpoint 2");
        }
    }
}
