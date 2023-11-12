/* 
 * Defines a MediatR command handler for processing group invitation acceptance requests.
 * This handler is tasked with invoking the appropriate group management service to handle the acceptance of group invitations, as indicated by the provided command data.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.GroupManagment;
using SafeShare.Utilities.Responses;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.GroupManagment;

/// <summary>
/// A MediatR command handler for processing group invitation acceptance requests.
/// </summary>
public class MediatR_AcceptInvitationRequestCommandHandler :
    MediatR_GenericHandler<IGroupManagment_GroupInvitationsRepository>,
    IRequestHandler<MediatR_AcceptInvitationRequestCommand, ObjectResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_AcceptInvitationRequestCommandHandler"/> class.
    /// </summary>
    /// <param name="service">The group management service used for processing group invitation acceptance.</param>
    public MediatR_AcceptInvitationRequestCommandHandler
    (
        IGroupManagment_GroupInvitationsRepository service
    )
    : base
    (
        service
    )
    { }
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