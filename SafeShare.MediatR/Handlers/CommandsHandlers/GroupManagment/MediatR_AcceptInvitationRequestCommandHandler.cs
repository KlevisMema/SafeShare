/* 
 * Defines a MediatR command handler for processing group invitation acceptance requests.
 * This handler is tasked with invoking the appropriate group management service to handle the acceptance of group invitations, as indicated by the provided command data.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.GroupManagment;
using SafeShare.Utilities.SafeShareApi.Responses;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.GroupManagment;

/// <summary>
/// A MediatR command handler for processing group invitation acceptance requests.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_AcceptInvitationRequestCommandHandler"/> class.
/// </remarks>
/// <param name="service">The group management service used for processing group invitation acceptance.</param>
public class MediatR_AcceptInvitationRequestCommandHandler
(
    IGroupManagment_GroupInvitationsRepository service
) : MediatR_GenericHandler<IGroupManagment_GroupInvitationsRepository>(service),
    IRequestHandler<MediatR_AcceptInvitationRequestCommand, ObjectResult>
{
    /// <summary>
    /// Handles the process of accepting a group invitation.
    /// </summary>
    /// <param name="request">The command containing the details of the group invitation to be accepted.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An ObjectResult with the outcome of the group invitation acceptance process.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_AcceptInvitationRequestCommand request,
        CancellationToken cancellationToken
    )
    {
        var acceptInvitationRequest = await _service.AcceptInvitation(request.DTO_AcceptInvitationRequest);

        return Util_GenericControllerResponse<bool>.ControllerResponse(acceptInvitationRequest);
    }
}