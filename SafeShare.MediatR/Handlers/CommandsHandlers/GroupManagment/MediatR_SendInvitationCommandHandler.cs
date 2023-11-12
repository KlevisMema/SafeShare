/* 
 * Defines a MediatR command handler for processing requests to send group invitations.
 * This handler is responsible for invoking the group management service to handle the sending of group invitations, based on the command's data.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.GroupManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.GroupManagment;

/// <summary>
/// A MediatR command handler for processing requests to send group invitations.
/// </summary>
public class MediatR_SendInvitationCommandHandler : 
    MediatR_GenericHandler<IGroupManagment_GroupInvitationsRepository>,
    IRequestHandler<MediatR_SendInvitationCommand, ObjectResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_SendInvitationCommandHandler"/> class.
    /// </summary>
    /// <param name="service">The group management service used for sending group invitations.</param>
    public MediatR_SendInvitationCommandHandler
    (
        IGroupManagment_GroupInvitationsRepository service
    )
    : base(service)
    { }
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