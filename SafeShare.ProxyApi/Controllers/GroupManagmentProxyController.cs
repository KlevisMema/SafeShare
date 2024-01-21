using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SafeShare.ProxyApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using SafeShare.ClientServerShared.Routes;
using Microsoft.AspNetCore.Http.HttpResults;
using SafeShare.ProxyApi.Container.Services;
using SafeShare.Security.JwtSecurity.Helpers;
using SafeShare.ProxyApi.Container.Interfaces;
using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment;
using SafeShare.DataTransormObject.SafeShareApi.Authentication;
using SafeShare.DataTransormObject.SafeShareApi.ExpenseManagment;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment.GroupInvitations;

namespace SafeShare.ProxyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "Default")]
public class GroupManagmentProxyController(IGroupManagmentProxyService groupManagmentProxyService) : ControllerBase
{
    [HttpGet(Route_GroupManagmentRoutes.ProxyGroupTypes)]
    public async Task<ActionResult<Util_GenericResponse<DTO_GroupsTypes>>>
    GetGroupsTypes()
    {
        var result = await groupManagmentProxyService.GetGroupTypes
        (
            API_Helper_ExtractInfoFromRequestCookie.UserId(API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request)),
            API_Helper_ExtractInfoFromRequestCookie.GetUserIp(Request),
            API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request)
        );

        SetCookiesInResponse(result.Item2);

        return Util_GenericControllerResponse<DTO_GroupsTypes>.ControllerResponse(result.Item1);
    }

    [HttpGet(Route_GroupManagmentRoutes.ProxyGetGroupDetails)]
    public async Task<ActionResult<Util_GenericResponse<DTO_GroupDetails>>>
    GetGroupDetails
    (
        Guid groupId
    )
    {
        var result = await groupManagmentProxyService.GetGroupDetails
        (
            API_Helper_ExtractInfoFromRequestCookie.UserId(API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request)),
            API_Helper_ExtractInfoFromRequestCookie.GetUserIp(Request),
            API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request),
            groupId
        );

        return Util_GenericControllerResponse<DTO_GroupDetails>.ControllerResponse(result);
    }

    [HttpPost(Route_GroupManagmentRoutes.ProxyCreateGroup)]
    public async Task<ActionResult<Util_GenericResponse<DTO_GroupType>>>
    CreateGroup
    (
        [FromForm] DTO_CreateGroup createGroup
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await groupManagmentProxyService.CreateGroup
        (
            API_Helper_ExtractInfoFromRequestCookie.UserId(API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request)),
            API_Helper_ExtractInfoFromRequestCookie.GetUserIp(Request),
            API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request),
            API_Helper_ExtractInfoFromRequestCookie.GetForgeryToken(Request),
            API_Helper_ExtractInfoFromRequestCookie.GetAspNetCoreForgeryToken(Request),
            createGroup
        );

        return Util_GenericControllerResponse<DTO_GroupType>.ControllerResponse(result);
    }

    [HttpPut(Route_GroupManagmentRoutes.ProxyEditGroup)]
    public async Task<ActionResult<Util_GenericResponse<DTO_GroupType>>>
    EditGroup
    (
        Guid groupId,
        [FromForm] DTO_EditGroup editGroup
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await groupManagmentProxyService.EditGroup
        (
            API_Helper_ExtractInfoFromRequestCookie.UserId(API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request)),
            API_Helper_ExtractInfoFromRequestCookie.GetUserIp(Request),
            API_Helper_ExtractInfoFromRequestCookie.GetForgeryToken(Request),
            API_Helper_ExtractInfoFromRequestCookie.GetAspNetCoreForgeryToken(Request),
            API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request),
            groupId,
            editGroup
        );

        return Util_GenericControllerResponse<DTO_GroupType>.ControllerResponse(result);
    }

    [HttpDelete(Route_GroupManagmentRoutes.ProxyDeleteGroup)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    DeleteGroup
    (
        Guid groupId
    )
    {
        var result = await groupManagmentProxyService.DeleteGroup
        (
            API_Helper_ExtractInfoFromRequestCookie.UserId(API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request)),
            API_Helper_ExtractInfoFromRequestCookie.GetUserIp(Request),
            API_Helper_ExtractInfoFromRequestCookie.GetForgeryToken(Request),
            API_Helper_ExtractInfoFromRequestCookie.GetAspNetCoreForgeryToken(Request),
            API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request),
            groupId
        );

        return Util_GenericControllerResponse<bool>.ControllerResponse(result);
    }

    [HttpGet(Route_GroupManagmentRoutes.ProxyGetGroupsInvitations)]
    public async Task<ActionResult<Util_GenericResponse<List<DTO_RecivedInvitations>>>>
    GetGroupsInvitations()
    {
        var result = await groupManagmentProxyService.GetGroupsInvitations
        (
            API_Helper_ExtractInfoFromRequestCookie.UserId(API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request)),
            API_Helper_ExtractInfoFromRequestCookie.GetUserIp(Request),
            API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request)
        );

        return Util_GenericControllerResponse<List<DTO_RecivedInvitations>>.ControllerResponse(result);
    }

    [HttpGet(Route_GroupManagmentRoutes.ProxyGetSentGroupInvitations)]
    public async Task<ActionResult<Util_GenericResponse<List<DTO_SentInvitations>>>>
    GetSentGroupInvitations()
    {
        var result = await groupManagmentProxyService.GetSentGroupInvitations
        (
            API_Helper_ExtractInfoFromRequestCookie.UserId(API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request)),
            API_Helper_ExtractInfoFromRequestCookie.GetUserIp(Request),
            API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request)
        );

        return Util_GenericControllerResponse<List<DTO_SentInvitations>>.ControllerResponse(result);
    }

    [HttpPost(Route_GroupManagmentRoutes.ProxySendInvitation)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    SendInvitation
    (
        DTO_SendInvitationRequest dTO_SendInvitation
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await groupManagmentProxyService.SendInvitation
        (
            API_Helper_ExtractInfoFromRequestCookie.UserId(API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request)),
            API_Helper_ExtractInfoFromRequestCookie.GetUserIp(Request),
            API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request),
            API_Helper_ExtractInfoFromRequestCookie.GetForgeryToken(Request),
            API_Helper_ExtractInfoFromRequestCookie.GetAspNetCoreForgeryToken(Request),
            dTO_SendInvitation
        );

        return Util_GenericControllerResponse<bool>.ControllerResponse(result);
    }

    [HttpPost(Route_GroupManagmentRoutes.ProxyAcceptInvitation)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    AcceptInvitation
    (
        DTO_InvitationRequestActions acceptInvitationRequest
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await groupManagmentProxyService.AcceptInvitation
        (
            API_Helper_ExtractInfoFromRequestCookie.UserId(API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request)),
            API_Helper_ExtractInfoFromRequestCookie.GetUserIp(Request),
            API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request),
            API_Helper_ExtractInfoFromRequestCookie.GetForgeryToken(Request),
            API_Helper_ExtractInfoFromRequestCookie.GetAspNetCoreForgeryToken(Request),
            acceptInvitationRequest
        );

        return Util_GenericControllerResponse<bool>.ControllerResponse(result);
    }

    [HttpPost(Route_GroupManagmentRoutes.ProxyRejectInvitation)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    RejectInvitation
    (
        DTO_InvitationRequestActions rejectInvitationRequest
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await groupManagmentProxyService.RejectInvitation
        (

            API_Helper_ExtractInfoFromRequestCookie.UserId(API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request)),
            API_Helper_ExtractInfoFromRequestCookie.GetUserIp(Request),
            API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request),
            API_Helper_ExtractInfoFromRequestCookie.GetForgeryToken(Request),
            API_Helper_ExtractInfoFromRequestCookie.GetAspNetCoreForgeryToken(Request),
            rejectInvitationRequest
        );

        return Util_GenericControllerResponse<bool>.ControllerResponse(result);
    }

    [HttpDelete(Route_GroupManagmentRoutes.ProxyDeleteInvitation)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    DeleteInvitation
    (
        DTO_InvitationRequestActions deleteInvitationRequest
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await groupManagmentProxyService.DeleteInvitation
        (
            API_Helper_ExtractInfoFromRequestCookie.UserId(API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request)),
            API_Helper_ExtractInfoFromRequestCookie.GetUserIp(Request),
            API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request),
            API_Helper_ExtractInfoFromRequestCookie.GetForgeryToken(Request),
            API_Helper_ExtractInfoFromRequestCookie.GetAspNetCoreForgeryToken(Request),
            deleteInvitationRequest
        );

        return Util_GenericControllerResponse<bool>.ControllerResponse(result);
    }

    [HttpDelete(Route_GroupManagmentRoutes.ProxyDeleteUsersFromGroup)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    DeleteUsersFromGroup
    (
        Guid groupId,
        List<DTO_UsersGroupDetails> UsersToRemoveFromGroup
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await groupManagmentProxyService.DeleteUsersFromGroup
        (
            API_Helper_ExtractInfoFromRequestCookie.UserId(API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request)),
            API_Helper_ExtractInfoFromRequestCookie.GetUserIp(Request),
            API_Helper_ExtractInfoFromRequestCookie.JwtToken(Request),
            API_Helper_ExtractInfoFromRequestCookie.GetForgeryToken(Request),
            API_Helper_ExtractInfoFromRequestCookie.GetAspNetCoreForgeryToken(Request),
            groupId,
            UsersToRemoveFromGroup
        );

        return Util_GenericControllerResponse<bool>.ControllerResponse(result);
    }

    private void
    SetCookiesInResponse
    (
        HttpResponseMessage httpResponseMessage
    )
    {
        if (httpResponseMessage.Headers.Contains("Set-Cookie"))
        {
            var cookies = httpResponseMessage.Headers.GetValues("Set-Cookie");
            foreach (var cookie in cookies)
                HttpContext.Response.Headers.Append("Set-Cookie", cookie);
        }
    }
}