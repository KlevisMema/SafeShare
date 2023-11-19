/* 
 * Defines a MediatR command handler for processing requests to reject group invitations.
 * This handler is responsible for invoking the group management service to handle the rejection of group invitations, based on the command's data.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.GroupManagment;
using SafeShare.Utilities.Responses;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.GroupManagment;

/// <summary>
/// A MediatR command handler for processing requests to reject group invitations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_RejectInvitationRequestCommandHandler"/> class.
/// </remarks>
/// <param name="service">The group management service used for rejecting group invitations.</param>
public class MediatR_RejectInvitationRequestCommandHandler
(
    IGroupManagment_GroupInvitationsRepository service
) : MediatR_GenericHandler<IGroupManagment_GroupInvitationsRepository>(service),
    IRequestHandler<MediatR_RejectInvitationRequestCommand, ObjectResult>

{
    /// <summary>
    /// Handles the process of rejecting a group invitation.
    /// </summary>
    /// <param name="request">The command containing the details of the group invitation to be rejected.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An ObjectResult with the outcome of the group invitation rejection process.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_RejectInvitationRequestCommand request,
        CancellationToken cancellationToken
    )
    {
        var rejectInvitationRequest = await _service.RejectInvitation(request.RejectInvitationRequest);

        return Util_GenericControllerResponse<bool>.ControllerResponse(rejectInvitationRequest);
    }
}