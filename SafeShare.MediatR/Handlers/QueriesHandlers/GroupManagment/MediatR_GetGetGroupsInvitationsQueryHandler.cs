/* 
 * Defines a MediatR query handler for retrieving group invitations.
 * This handler is responsible for querying the group management service to retrieve a list of received group invitations for a specific user.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.MediatR.Actions.Queries.GroupManagment;
using SafeShare.DataTransormObject.GroupManagment.GroupInvitations;


namespace SafeShare.MediatR.Handlers.QueriesHandlers.GroupManagment;

/// <summary>
/// A MediatR query handler for retrieving group invitations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_GetGetGroupsInvitationsQueryHandler"/> class.
/// </remarks>
/// <param name="service">The group management service used for retrieving group invitations.</param>
public class MediatR_GetGetGroupsInvitationsQueryHandler
(
    IGroupManagment_GroupInvitationsRepository service
) : MediatR_GenericHandler<IGroupManagment_GroupInvitationsRepository>(service),
    IRequestHandler<MediatR_GetGetGroupsInvitationsQuery, ObjectResult>
{
    /// <summary>
    /// Handles the process of retrieving a list of received group invitations for a specific user.
    /// </summary>
    /// <param name="request">The query containing the user ID for whom the invitations are to be retrieved.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An ObjectResult with the list of received group invitations.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_GetGetGroupsInvitationsQuery request,
        CancellationToken cancellationToken
    )
    {
        var getGroupsInvitationsResult = await _service.GetRecivedGroupsInvitations(request.UserId);

        return Util_GenericControllerResponse<DTO_RecivedInvitations>.ControllerResponseList(getGroupsInvitationsResult);
    }
}