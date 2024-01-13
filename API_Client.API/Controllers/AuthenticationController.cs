using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using API_Client.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using SafeShare.DataTransormObject.SafeShareApiKey.Authentication;

namespace API_Client.API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult>
    Login
    (
        [FromForm] DTO_LoginRequest loginRequest
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await authenticationService.Login(loginRequest.UserName, loginRequest.Password);

        if (result.Success)
            return Ok(result);

        return BadRequest(result.Message);
    }
}