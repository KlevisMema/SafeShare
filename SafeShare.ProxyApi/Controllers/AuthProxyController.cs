using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using SafeShare.ClientServerShared.Routes;
using Microsoft.AspNetCore.Http.HttpResults;
using SafeShare.ProxyApi.Container.Services;
using Microsoft.AspNetCore.Components.Forms;
using SafeShare.Security.JwtSecurity.Helpers;
using SafeShare.ProxyApi.Container.Interfaces;
using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.DataTransormObject.SafeShareApi.Security;
using SafeShare.DataTransormObject.SafeShareApi.Authentication;

namespace SafeShare.ProxyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthProxyController(IProxyAuthentication authenticationService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost(Route_AuthenticationRoute.Register)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    Register
    (
        [FromForm] DTO_Register register
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await authenticationService.RegisterUser(register);

        return Util_GenericControllerResponse<bool>.ControllerResponse(result);
    }

    [AllowAnonymous]
    [HttpPost(Route_AuthenticationRoute.ConfirmRegistration)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    ConfirmRegistration
    (
        [FromBody] DTO_ConfirmRegistration confirmRegistrationDto
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await authenticationService.ConfirmRegistration(confirmRegistrationDto);

        return Util_GenericControllerResponse<bool>.ControllerResponse(result);
    }

    [AllowAnonymous]
    [HttpPost(Route_AuthenticationRoute.ReConfirmRegistrationRequest)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    ReConfirmRegistrationRequest
    (
       [FromBody] DTO_ReConfirmRegistration reConfirmRegistration
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await authenticationService.ReConfirmRegistrationRequest(reConfirmRegistration);

        return Util_GenericControllerResponse<bool>.ControllerResponse(result);
    }

    [AllowAnonymous]
    [HttpPost(Route_AuthenticationRoute.Login)]
    public async Task<ActionResult<Util_GenericResponse<DTO_LoginResult>>>
    LoginUser
    (
        [FromForm] DTO_Login loginDto
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await authenticationService.LogIn(loginDto);

        if (result.Item2.Headers.Contains("Set-Cookie"))
        {
            var cookies = result.Item2.Headers.GetValues("Set-Cookie");
            foreach (var cookie in cookies)
                HttpContext.Response.Headers.Append("Set-Cookie", cookie);
        }

        return Util_GenericControllerResponse<DTO_LoginResult>.ControllerResponse(result.Item1);
    }

    [HttpPost(Route_AuthenticationRoute.ConfirmLogin)]
    public async Task<ActionResult<Util_GenericResponse<DTO_LoginResult>>>
    ConfirmLogin
    (
        Guid userId,
        DTO_ConfirmLogin confirmLogin
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var jwtToken = Request.Cookies["AuthToken"] ?? string.Empty;

        var result = await authenticationService.ConfirmLogin2FA(userId, jwtToken, confirmLogin);

        if (result.Item2.Headers.Contains("Set-Cookie"))
        {
            var cookies = result.Item2.Headers.GetValues("Set-Cookie");
            foreach (var cookie in cookies)
                HttpContext.Response.Headers.Append("Set-Cookie", cookie);
        }

        return Util_GenericControllerResponse<DTO_LoginResult>.ControllerResponse(result.Item1);
    }

    [HttpPost(Route_AuthenticationRoute.LogOut)]
    [Authorize(AuthenticationSchemes = "Default")]
    public async Task<ActionResult>
    LogOut
    (
        Guid userId
    )
    {
        var jwtToken = Request.Cookies["AuthToken"] ?? string.Empty;

        var result = await authenticationService.LogoutUser(userId, jwtToken);

        if (result.Headers.Contains("Set-Cookie"))
        {
            var cookies = result.Headers.GetValues("Set-Cookie");
            foreach (var cookie in cookies)
                HttpContext.Response.Headers.Append("Set-Cookie", cookie);
        }

        return Ok();
    }

    [AllowAnonymous]
    [HttpPost(Route_AuthenticationRoute.RefreshToken)]
    public async Task<ActionResult<Util_GenericResponse<DTO_Token>>>
    RefreshToken()
    {
        var jwtToken = Request.Cookies["AuthToken"] ?? string.Empty;
        var refreshToken = Request.Cookies["RefreshAuthToken"] ?? string.Empty;
        var refreshTokenId = Request.Cookies["RefreshAuthTokenId"] ?? string.Empty;

        var result = await authenticationService.RefreshToken(jwtToken, refreshToken, refreshTokenId);

        if (result.Item2.Headers.Contains("Set-Cookie"))
        {
            var cookies = result.Item2.Headers.GetValues("Set-Cookie");
            foreach (var cookie in cookies)
                HttpContext.Response.Headers.Append("Set-Cookie", cookie);
        }

        return Util_GenericControllerResponse<DTO_Token>.ControllerResponse(result.Item1);
    }
}