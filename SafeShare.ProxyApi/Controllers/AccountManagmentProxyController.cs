﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using SafeShare.ClientServerShared.Routes;
using SafeShare.Security.API.ActionFilters;
using Microsoft.AspNetCore.Http.HttpResults;
using SafeShare.Security.JwtSecurity.Helpers;
using SafeShare.ProxyApi.Container.Interfaces;
using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.DataTransormObject.SafeShareApi.UserManagment;
using SafeShare.DataTransormObject.SafeShareApi.Authentication;

namespace SafeShare.ProxyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "Default")]
public class AccountManagmentProxyController(IAccountManagmentProxyService accountManagmentProxyService) : ControllerBase
{
    [HttpGet(Route_AccountManagmentRoute.ProxyGetUser)]
    public async Task<ActionResult<Util_GenericResponse<DTO_UserUpdatedInfo>>>
    GetUser()
    {
        var jwtToken = JwtToken();
        var result = await accountManagmentProxyService.GetUser(UserId(jwtToken), jwtToken);
        return Util_GenericControllerResponse<DTO_UserUpdatedInfo>.ControllerResponse(result);
    }

    [HttpPut(Route_AccountManagmentRoute.ProxyUpdateUser)]
    public async Task<ActionResult<Util_GenericResponse<DTO_UserUpdatedInfo>>>
    UpdateUser
    (
        [FromForm] DTO_UserInfo userInfo
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var jwtToken = JwtToken();
        var result = await accountManagmentProxyService.UpdateUser(UserId(jwtToken), jwtToken, userInfo);

        if (result.Item2.Headers.Contains("Set-Cookie"))
        {
            var cookies = result.Item2.Headers.GetValues("Set-Cookie");
            foreach (var cookie in cookies)
                HttpContext.Response.Headers.Append("Set-Cookie", cookie);
        }

        return Util_GenericControllerResponse<DTO_UserUpdatedInfo>.ControllerResponse(result.Item1);
    }

    [HttpPut(Route_AccountManagmentRoute.ProxyChangePassword)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    ChangePassword
    (
        [FromForm] DTO_UserChangePassword changePassword
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);


        var jwtToken = JwtToken();

        var result = await accountManagmentProxyService.ChangePassword(UserId(jwtToken), jwtToken, changePassword);

        return Util_GenericControllerResponse<bool>.ControllerResponse(result);
    }

    [HttpPost(Route_AccountManagmentRoute.ProxyDeactivateAccount)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    DeactivateAccount
    (
        [FromForm] DTO_DeactivateAccount deactivateAccount
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var jwtToken = JwtToken();

        var result = await accountManagmentProxyService.DeactivateAccount(UserId(jwtToken), jwtToken, deactivateAccount);

        if (result.Item2.Headers.Contains("Set-Cookie"))
        {
            var cookies = result.Item2.Headers.GetValues("Set-Cookie");
            foreach (var cookie in cookies)
                HttpContext.Response.Headers.Append("Set-Cookie", cookie);
        }

        return Util_GenericControllerResponse<bool>.ControllerResponse(result.Item1);
    }

    [AllowAnonymous]
    [HttpPost(Route_AccountManagmentRoute.ActivateAccountRequest)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    ActivateAccountRequest
    (
        string email
    )
    {
        var result = await accountManagmentProxyService.ActivateAccountRequest(email);

        return Util_GenericControllerResponse<bool>.ControllerResponse(result);
    }

    [AllowAnonymous]
    [HttpPost(Route_AccountManagmentRoute.ActivateAccountRequestConfirmation)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    ActivateAccountRequestConfirmation
    (
        DTO_ActivateAccountConfirmation activateAccountConfirmationDto
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await accountManagmentProxyService.ActivateAccountRequestConfirmation(activateAccountConfirmationDto);

        return Util_GenericControllerResponse<bool>.ControllerResponse(result);
    }

    [AllowAnonymous]
    [HttpPost(Route_AccountManagmentRoute.ForgotPassword)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    ForgotPassword
    (
       [FromForm] DTO_ForgotPassword forgotPassword
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await accountManagmentProxyService.ForgotPassword(forgotPassword);

        return Util_GenericControllerResponse<bool>.ControllerResponse(result);
    }


    [AllowAnonymous]
    [HttpPost(Route_AccountManagmentRoute.ResetPassword)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    ResetPassword
    (
        [FromForm] DTO_ResetPassword resetPassword
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await accountManagmentProxyService.ResetPassword(resetPassword);

        return Util_GenericControllerResponse<bool>.ControllerResponse(result);
    }

    [HttpPost(Route_AccountManagmentRoute.ProxyRequestChangeEmail)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    RequestChangeEmail
    (
        [FromForm] DTO_ChangeEmailAddressRequest emailAddress
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var jwtToken = JwtToken();

        var result = await accountManagmentProxyService.RequestChangeEmail(UserId(jwtToken), jwtToken, emailAddress);

        return Util_GenericControllerResponse<bool>.ControllerResponse(result);
    }

    [HttpPost(Route_AccountManagmentRoute.ProxyConfirmChangeEmailRequest)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    ConfirmChangeEmailAddressRequest
    (
        DTO_ChangeEmailAddressRequestConfirm changeEmailAddressConfirmDto
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var jwtToken = JwtToken();

        var result = await accountManagmentProxyService.ConfirmChangeEmailAddressRequest(UserId(jwtToken), jwtToken, changeEmailAddressConfirmDto);

        if (result.Item2.Headers.Contains("Set-Cookie"))
        {
            var cookies = result.Item2.Headers.GetValues("Set-Cookie");
            foreach (var cookie in cookies)
                HttpContext.Response.Headers.Append("Set-Cookie", cookie);
        }

        return Util_GenericControllerResponse<bool>.ControllerResponse(result.Item1);
    }

    [HttpGet(Route_AccountManagmentRoute.ProxySearchUserByUserName)]
    public async Task<ActionResult<Util_GenericResponse<List<DTO_UserSearched>>>>
    SearchUserByUserName
    (
        string userName,
        CancellationToken cancellationToken
    )
    {
        var jwtToken = JwtToken();

        var result = await accountManagmentProxyService.SearchUserByUserName(UserId(jwtToken), jwtToken, userName, cancellationToken);

        return Util_GenericControllerResponse<DTO_UserSearched>.ControllerResponseList(result);
    }

    [HttpPost(Route_AccountManagmentRoute.ProxyUploadProfilePicture)]
    public async Task<ActionResult<Util_GenericResponse<byte[]>>>
    UploadProfilePicture
    (
        [FromForm] IFormFile image
    )
    {
        var jwtToken = JwtToken();

        var result = await accountManagmentProxyService.UploadProfilePicture(UserId(jwtToken), jwtToken, image);

        return Util_GenericControllerResponse<byte[]>.ControllerResponse(result);
    }

    private static string
    UserId
    (
        string jwtToken
    )
    {
        return Helper_JwtToken.GetUserIdDirectlyFromJwtToken(jwtToken);
    }

    private static string
    UserEmail
    (
        string jwtToken
    )
    {
        return Helper_JwtToken.GetUserEmailDirectlyFromJwtToken(jwtToken);
    }

    private string
    JwtToken()
    {
        return Request.Cookies["AuthToken"] ?? string.Empty;
    }
}