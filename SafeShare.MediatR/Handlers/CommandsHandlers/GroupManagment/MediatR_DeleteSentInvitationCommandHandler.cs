﻿/* 
 * Defines a MediatR command handler for processing requests to delete sent group invitations.
 * This handler is tasked with invoking the group management service to execute the deletion of invitations sent by a user, based on the provided command data.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.GroupManagment;
using SafeShare.Utilities.SafeShareApi.Responses;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.GroupManagment;

/// <summary>
/// A MediatR command handler for processing requests to delete sent group invitations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_DeleteSentInvitationCommandHandler"/> class.
/// </remarks>
/// <param name="service">The group management service used for deleting sent invitations.</param>
public class MediatR_DeleteSentInvitationCommandHandler
(
    IGroupManagment_GroupInvitationsRepository service
) : MediatR_GenericHandler<IGroupManagment_GroupInvitationsRepository>(service),
    IRequestHandler<MediatR_DeleteSentInvitationCommand, ObjectResult>
{
    /// <summary>
    /// Handles the process of deleting a sent group invitation.
    /// </summary>
    /// <param name="request">The command containing the details of the sent invitation to be deleted.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An ObjectResult with the outcome of the sent invitation deletion process.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_DeleteSentInvitationCommand request,
        CancellationToken cancellationToken
    )
    {
        var deleteInvitationResult = await _service.DeleteSentInvitation(request.DeleteInvitationRequest);

        return Util_GenericControllerResponse<bool>.ControllerResponse(deleteInvitationResult);
    }
}