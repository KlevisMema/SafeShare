/* 
 * Defines a MediatR query handler for retrieving detailed information about a specific group.
 * This handler is responsible for querying the group management service to obtain detailed data of a group, including its members and activities, based on the group ID.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.MediatR.Actions.Queries.GroupManagment;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment;
using SafeShare.Utilities.SafeShareApi.Responses;

namespace SafeShare.MediatR.Handlers.QueriesHandlers.GroupManagment;

/// <summary>
/// A MediatR query handler for retrieving detailed information about a specific group.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_GetGroupDetailsQueryHandler"/> class.
/// </remarks>
/// <param name="service">The group management service used for retrieving group details.</param>
public class MediatR_GetGroupDetailsQueryHandler
(
    IGroupManagment_GroupRepository service
) : MediatR_GenericHandler<IGroupManagment_GroupRepository>(service),
    IRequestHandler<MediatR_GetGroupDetailsQuery, ObjectResult>
{
    /// <summary>
    /// Handles the process of retrieving detailed information about a specific group.
    /// </summary>
    /// <param name="request">The query containing the user ID and group ID for which the details are to be retrieved.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An ObjectResult with detailed information about the specified group.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_GetGroupDetailsQuery request,
        CancellationToken cancellationToken
    )
    {
        var getGroupDetailsResult = await _service.GetGroupDetails(request.UserId, request.GroupId);

        return Util_GenericControllerResponse<DTO_GroupDetails>.ControllerResponse(getGroupDetailsResult);
    }
}