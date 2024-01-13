/* 
 * Defines a MediatR command for handling the editing of a group.
 * This command is used within MediatR handlers to manage the process of editing group details, such as the group's name or description.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment;

namespace SafeShare.MediatR.Actions.Commands.GroupManagment;

/// <summary>
/// Represents a MediatR command for editing the details of a group.
/// This command includes the identifiers for the group and the user, along with the details to be updated.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_EditGroupCommand"/> class.
/// </remarks>
/// <param name="userId">The unique identifier of the user performing the edit operation.</param>
/// <param name="groupId">The unique identifier of the group to be edited.</param>
/// <param name="editGroup">The DTO containing the new details for the group.</param>
public class MediatR_EditGroupCommand
(
    Guid userId,
    Guid groupId,
    DTO_EditGroup editGroup
) : IRequest<ObjectResult>
{
    /// <summary>
    /// The unique identifier of the user performing the edit operation.
    /// </summary>
    public Guid UserId { get; set; } = userId;
    /// <summary>
    /// The unique identifier of the group to be edited.
    /// </summary>
    public Guid GroupId { get; set; } = groupId;
    /// <summary>
    /// Data transfer object containing the new details for the group.
    /// </summary>
    public DTO_EditGroup EditGroup { get; set; } = editGroup;
}