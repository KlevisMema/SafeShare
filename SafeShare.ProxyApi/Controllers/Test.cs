using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SafeShare.ProxyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Test : ControllerBase
    {
        [HttpGet("yyyy")]
        [Authorize(AuthenticationSchemes = "Default")]
        public IActionResult Test1()
        {
            return Content("Hi");
        }
    }
}
