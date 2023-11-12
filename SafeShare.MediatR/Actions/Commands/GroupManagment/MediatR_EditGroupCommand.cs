/* 
 * Defines a MediatR command for handling the editing of a group.
 * This command is used within MediatR handlers to manage the process of editing group details, such as the group's name or description.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.GroupManagment;

namespace SafeShare.MediatR.Actions.Commands.GroupManagment;

/// <summary>
/// Represents a MediatR command for editing the details of a group.
/// This command includes the identifiers for the group and the user, along with the details to be updated.
/// </summary>
public class MediatR_EditGroupCommand : IRequest<ObjectResult>
{
    /// <summary>
    /// The unique identifier of the user performing the edit operation.
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// The unique identifier of the group to be edited.
    /// </summary>
    public Guid GroupId { get; set; }
    /// <summary>
    /// Data transfer object containing the new details for the group.
    /// </summary>
    public DTO_EditGroup EditGroup { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_EditGroupCommand"/> class.
    /// </summary>
    /// <param name="userId">The unique identifier of the user performing the edit operation.</param>
    /// <param name="groupId">The unique identifier of the group to be edited.</param>
    /// <param name="editGroup">The DTO containing the new details for the group.</param>
    public MediatR_EditGroupCommand
    (
        Guid userId,
        Guid groupId,
        DTO_EditGroup editGroup
    )
    {
        UserId = userId;
        GroupId = groupId;
        EditGroup = editGroup;
    }
}