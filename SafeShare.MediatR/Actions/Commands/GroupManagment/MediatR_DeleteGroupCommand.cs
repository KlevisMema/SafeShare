/* 
 * Defines a MediatR command for handling the deletion of a group.
 * This command is utilized within MediatR handlers to manage the process of deleting groups, with specific focus on ownership and group identification.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SafeShare.MediatR.Actions.Commands.GroupManagment;

/// <summary>
/// Represents a MediatR command for deleting a group.
/// This command includes the identifiers for the group and its owner, essential for processing the deletion request.
/// </summary>
public class MediatR_DeleteGroupCommand : IRequest<ObjectResult>
{
    /// <summary>
    /// The unique identifier of the group's owner.
    /// </summary>
    public Guid OwnerId { get; set; }
    /// <summary>
    /// The unique identifier of the group to be deleted.
    /// </summary>
    public Guid GroupId { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_DeleteGroupCommand"/> class.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the group's owner.</param>
    /// <param name="groupId">The unique identifier of the group to be deleted.</param>
    public MediatR_DeleteGroupCommand
    (
        Guid ownerId,
        Guid groupId
    )
    {
        OwnerId = ownerId;
        GroupId = groupId;
    }
}