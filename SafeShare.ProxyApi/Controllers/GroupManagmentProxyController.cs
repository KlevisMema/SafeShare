using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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
        var jwtToken = Request.Cookies["AuthToken"] ?? string.Empty;

        var result = await groupManagmentProxyService.GetGroupTypes(jwtToken);

        return Util_GenericControllerResponse<DTO_GroupsTypes>.ControllerResponse(result);
    }

    [HttpGet(Route_GroupManagmentRoutes.ProxyGetGroupDetails)]
    public async Task<ActionResult<Util_GenericResponse<DTO_GroupDetails>>>
    GetGroupDetails
    (
        Guid groupId
    )
    {
        var jwtToken = JwtToken();

        var result = await groupManagmentProxyService.GetGroupDetails(UserId(jwtToken), jwtToken, groupId);

        return Util_GenericControllerResponse<DTO_GroupDetails>.ControllerResponse(result);
    }

    [HttpPost(Route_GroupManagmentRoutes.ProxyCreateGroup)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    CreateGroup
    (
        [FromForm] DTO_CreateGroup createGroup
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var jwtToken = JwtToken();

        var result = await groupManagmentProxyService.CreateGroup(UserId(jwtToken), jwtToken, createGroup);

        return Util_GenericControllerResponse<bool>.ControllerResponse(result);
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

        var jwtToken = JwtToken();

        var result = await groupManagmentProxyService.EditGroup(UserId(jwtToken), jwtToken, groupId, editGroup);

        return Util_GenericControllerResponse<DTO_GroupType>.ControllerResponse(result);
    }

    [HttpDelete(Route_GroupManagmentRoutes.ProxyDeleteGroup)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    DeleteGroup
    (
        Guid groupId
    )
    {
        var jwtToken = JwtToken();

        var result = await groupManagmentProxyService.DeleteGroup(UserId(jwtToken), jwtToken, groupId);

        return Util_GenericControllerResponse<bool>.ControllerResponse(result);
    }

    [HttpGet(Route_GroupManagmentRoutes.ProxyGetGroupsInvitations)]
    public async Task<ActionResult<Util_GenericResponse<List<DTO_RecivedInvitations>>>>
    GetGroupsInvitations()
    {
        var jwtToken = JwtToken();

        var result = await groupManagmentProxyService.GetGroupsInvitations(UserId(jwtToken), jwtToken);

        return Util_GenericControllerResponse<List<DTO_RecivedInvitations>>.ControllerResponse(result);
    }

    [HttpGet(Route_GroupManagmentRoutes.ProxyGetSentGroupInvitations)]
    public async Task<ActionResult<Util_GenericResponse<List<DTO_SentInvitations>>>>
    GetSentGroupInvitations()
    {
        var jwtToken = JwtToken();

        var result = await groupManagmentProxyService.GetSentGroupInvitations(UserId(jwtToken), jwtToken);

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

        var jwtToken = JwtToken();

        var result = await groupManagmentProxyService.SendInvitation(UserId(jwtToken), jwtToken, dTO_SendInvitation);

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

        var jwtToken = JwtToken();

        var result = await groupManagmentProxyService.AcceptInvitation(UserId(jwtToken), jwtToken, acceptInvitationRequest);

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

        var jwtToken = JwtToken();

        var result = await groupManagmentProxyService.RejectInvitation(UserId(jwtToken), jwtToken, rejectInvitationRequest);

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

        var jwtToken = JwtToken();

        var result = await groupManagmentProxyService.DeleteInvitation(UserId(jwtToken), jwtToken, deleteInvitationRequest);

        return Util_GenericControllerResponse<bool>.ControllerResponse(result);
    }

    private static string
    UserId
    (
        string jwtToken
    )
    {
        return Helper_JwtToken.GetUserIdDirectlyFromJwtToken(jwtToken);
    }

    private string
    JwtToken()
    {
        return Request.Cookies["AuthToken"] ?? string.Empty;
    }
}