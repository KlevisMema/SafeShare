/* 
 * Defines a MediatR command handler for processing requests to send group invitations.
 * This handler is responsible for invoking the group management service to handle the sending of group invitations, based on the command's data.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.GroupManagment;
using SafeShare.Utilities.SafeShareApi.Responses;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.GroupManagment;

/// <summary>
/// A MediatR command handler for processing requests to send group invitations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_SendInvitationCommandHandler"/> class.
/// </remarks>
/// <param name="service">The group management service used for sending group invitations.</param>
public class MediatR_SendInvitationCommandHandler
(
    IGroupManagment_GroupInvitationsRepository service
) : MediatR_GenericHandler<IGroupManagment_GroupInvitationsRepository>(service),
    IRequestHandler<MediatR_SendInvitationCommand, ObjectResult>
{
    /// <summary>
    /// Handles the process of sending a group invitation.
    /// </summary>
    /// <param name="request">The command containing the details for the group invitation to be sent.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An ObjectResult with the outcome of the group invitation sending process.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_SendInvitationCommand request,
        CancellationToken cancellationToken
    )
    {
        var sendInvitationResult = await _service.SendInvitation(request.DTO_SendInvitation);

        return Util_GenericControllerResponse<bool>.ControllerResponse(sendInvitationResult);
    }
}