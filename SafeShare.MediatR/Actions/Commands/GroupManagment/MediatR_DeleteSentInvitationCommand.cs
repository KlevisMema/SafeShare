/* 
 * Defines a MediatR command for handling the deletion of a sent group invitation.
 * This command facilitates the process of deleting invitations that were previously sent, using MediatR handlers to process the request.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment.GroupInvitations;

namespace SafeShare.MediatR.Actions.Commands.GroupManagment;

/// <summary>
/// Represents a MediatR command for deleting a sent group invitation.
/// This command carries the necessary data for processing the deletion of an invitation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_DeleteSentInvitationCommand"/> class.
/// </remarks>
/// <param name="deleteInvitationRequest">The DTO containing data for the invitation deletion request.</param>
public class MediatR_DeleteSentInvitationCommand
(
    DTO_InvitationRequestActions deleteInvitationRequest
) : IRequest<ObjectResult>
{
    /// <summary>
    /// Data transfer object containing the information required for deleting a sent group invitation.
    /// </summary>
    public DTO_InvitationRequestActions DeleteInvitationRequest { get; set; } = deleteInvitationRequest;
}