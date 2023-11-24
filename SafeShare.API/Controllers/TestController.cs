using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Security.API.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace SafeShare.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult EncryptKey
        ()
        {
            var apiKey = Environment.GetEnvironmentVariable("SAFE_SHARE_API_KEY");
            var publicKey = Environment.GetEnvironmentVariable("SAFE_SHARE_API_PUBLIC_KEY");

            var bytesToEncrypt = Encoding.UTF8.GetBytes(apiKey);

            using var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);

            var encryptedData = rsa.Encrypt(bytesToEncrypt, true);
            return Ok(Convert.ToBase64String(encryptedData));
        }

        [HttpGet("yyyy")]
        [Authorize(AuthenticationSchemes = "Default")]
        //[ServiceFilter(typeof(IApiKeyAuthorizationFilter))]
        public IActionResult Test()
        {
            return Ok("hI");
        }
    }
}
