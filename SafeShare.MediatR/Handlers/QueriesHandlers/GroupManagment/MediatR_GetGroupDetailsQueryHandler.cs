/* 
 * Defines a MediatR query handler for retrieving detailed information about a specific group.
 * This handler is responsible for querying the group management service to obtain detailed data of a group, including its members and activities, based on the group ID.
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
/// A MediatR query handler for retrieving detailed information about a specific group.
/// </summary>
public class MediatR_GetGroupDetailsQueryHandler : 
    MediatR_GenericHandler<IGroupManagment_GroupRepository>, 
    IRequestHandler<MediatR_GetGroupDetailsQuery, ObjectResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_GetGroupDetailsQueryHandler"/> class.
    /// </summary>
    /// <param name="service">The group management service used for retrieving group details.</param>
    public MediatR_GetGroupDetailsQueryHandler
    (
        IGroupManagment_GroupRepository service
    )
    : base
    (
        service
    )
    { }
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