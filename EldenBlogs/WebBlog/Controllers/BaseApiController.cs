using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebBlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> BaseEndpoint()
        {
            return Ok("HI, I'm an Endpoint");
        }
    }
}
