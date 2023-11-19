/* 
 * Defines a MediatR query handler for retrieving sent group invitations by a specific user.
 * This handler is responsible for querying the group management service to obtain a list of group invitations that have been sent out by the user.
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
/// A MediatR query handler for retrieving sent group invitations by a specific user.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_GetSentGroupInvitationsQueryHandler"/> class.
/// </remarks>
/// <param name="service">The group management service used for retrieving sent group invitations.</param>
public class MediatR_GetSentGroupInvitationsQueryHandler
(
    IGroupManagment_GroupInvitationsRepository service
) : MediatR_GenericHandler<IGroupManagment_GroupInvitationsRepository>(service),
    IRequestHandler<MediatR_GetSentGroupInvitationsQuery, ObjectResult>
{
    /// <summary>
    /// Handles the process of retrieving a list of group invitations that have been sent out by a specific user.
    /// </summary>
    /// <param name="request">The query containing the user ID for whom the sent invitations are to be retrieved.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An ObjectResult with the list of sent group invitations.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_GetSentGroupInvitationsQuery request,
        CancellationToken cancellationToken
    )
    {
        var getSentInvitationsResult = await _service.GetSentGroupInvitations(request.UserId);

        return Util_GenericControllerResponse<DTO_SentInvitations>.ControllerResponseList(getSentInvitationsResult);
    }
}
