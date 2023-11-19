/* 
 * Defines a MediatR command for handling the acceptance of group invitations.
 * This command is utilized within MediatR handlers to manage the process of accepting invitations to groups.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.GroupManagment.GroupInvitations;

namespace SafeShare.MediatR.Actions.Commands.GroupManagment;

/// <summary>
/// Represents a MediatR command for handling the acceptance of a group invitation.
/// This command carries the necessary data for processing an invitation acceptance.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_AcceptInvitationRequestCommand"/> class.
/// </remarks>
/// <param name="dTO_AcceptInvitationRequest">The DTO containing data for the invitation acceptance request.</param>
public class MediatR_AcceptInvitationRequestCommand
(
    DTO_InvitationRequestActions dTO_AcceptInvitationRequest
) : IRequest<ObjectResult>
{
    /// <summary>
    /// Data transfer object containing the information required for accepting a group invitation.
    /// </summary>
    public DTO_InvitationRequestActions DTO_AcceptInvitationRequest { get; set; } = dTO_AcceptInvitationRequest;
}