/* 
 * Defines a MediatR query handler for retrieving types of groups associated with a specific user.
 * This handler is responsible for querying the group management service to obtain a list of groups that the user has joined or created.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.DataTransormObject.GroupManagment;
using SafeShare.MediatR.Actions.Queries.GroupManagment;

namespace SafeShare.MediatR.Handlers.QueriesHandlers.GroupManagment;

/// <summary>
/// A MediatR query handler for retrieving types of groups associated with a specific user.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_GetGroupsTypesQueryHandler"/> class.
/// </remarks>
/// <param name="service">The group management service used for retrieving types of groups associated with a user.</param>
public class MediatR_GetGroupsTypesQueryHandler
(
    IGroupManagment_GroupRepository service
) : MediatR_GenericHandler<IGroupManagment_GroupRepository>(service),
    IRequestHandler<MediatR_GetGroupsTypesQuery, ObjectResult>
{
    /// <summary>
    /// Handles the process of retrieving types of groups associated with a specific user.
    /// </summary>
    /// <param name="request">The query containing the user ID for whom the group types are to be retrieved.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An ObjectResult with the types of groups associated with the specified user.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_GetGroupsTypesQuery request,
        CancellationToken cancellationToken
    )
    {
        var getGroupsTypesResult = await _service.GetGroupsTypes(request.UserId);

        return Util_GenericControllerResponse<DTO_GroupsTypes>.ControllerResponse(getGroupsTypesResult);
    }
}