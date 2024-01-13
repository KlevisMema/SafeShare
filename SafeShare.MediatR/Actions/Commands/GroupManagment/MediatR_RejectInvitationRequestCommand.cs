/* 
 * Defines a MediatR command for handling the rejection of a group invitation.
 * This command is utilized within MediatR handlers to manage the process of rejecting invitations to join groups, encapsulating the necessary data for the rejection action.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment.GroupInvitations;

namespace SafeShare.MediatR.Actions.Commands.GroupManagment;

/// <summary>
/// Represents a MediatR command for rejecting a group invitation.
/// This command carries the necessary data for processing the rejection of an invitation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_RejectInvitationRequestCommand"/> class.
/// </remarks>
/// <param name="rejectInvitationRequest">The DTO containing data for the invitation rejection request.</param>
public class MediatR_RejectInvitationRequestCommand
(
    DTO_InvitationRequestActions rejectInvitationRequest
) : IRequest<ObjectResult>
{
    /// <summary>
    /// Data transfer object containing the information required for rejecting a group invitation.
    /// </summary>
    public DTO_InvitationRequestActions RejectInvitationRequest { get; set; } = rejectInvitationRequest;
}