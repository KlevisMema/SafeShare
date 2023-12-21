using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using SafeShare.ClientServerShared.Routes;
using SafeShare.ProxyApi.Container.Interfaces;
using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment;
using SafeShare.DataTransormObject.SafeShareApi.Authentication;

namespace SafeShare.ProxyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "Default")]
public class GroupManagmentProxyController(IGroupManagmentProxyService groupManagmentProxyService) : ControllerBase
{

    [HttpGet("GroupTypes")]
    public async Task<ActionResult<Util_GenericResponse<DTO_GroupsTypes>>>
    GetGroupsTypes()
    {
        var jwtToken = Request.Cookies["AuthToken"] ?? string.Empty;

        var result = await groupManagmentProxyService.GetGroupTypes(jwtToken);

        return Util_GenericControllerResponse<DTO_GroupsTypes>.ControllerResponse(result);
    }
}