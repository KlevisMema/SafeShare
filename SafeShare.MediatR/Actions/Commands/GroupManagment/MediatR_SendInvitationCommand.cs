/* 
 * Defines a MediatR command for sending a group invitation.
 * This command is used within MediatR handlers to facilitate the process of sending invitations to join groups, incorporating the necessary details for the invitation.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment.GroupInvitations;

namespace SafeShare.MediatR.Actions.Commands.GroupManagment;

/// <summary>
/// Represents a MediatR command for sending a group invitation.
/// This command includes the data required for issuing an invitation to join a group.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_SendInvitationCommand"/> class.
/// </remarks>
/// <param name="dTO_SendInvitation">The DTO containing data for sending the group invitation.</param>
public class MediatR_SendInvitationCommand
(
    DTO_SendInvitationRequest dTO_SendInvitation
) : IRequest<ObjectResult>
{
    /// <summary>
    /// Data transfer object containing the information required for sending a group invitation.
    /// </summary>
    public DTO_SendInvitationRequest DTO_SendInvitation { get; set; } = dTO_SendInvitation;
}