﻿/* 
 * Defines a MediatR command for creating a new group.
 * This command is used within MediatR handlers to facilitate the group creation process, involving data like the group's owner and its details.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment;

namespace SafeShare.MediatR.Actions.Commands.GroupManagment;

/// <summary>
/// Represents a MediatR command for creating a new group.
/// This command includes the owner's identifier and the necessary data for group creation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_CreateGroupCommand"/> class.
/// </remarks>
/// <param name="ownerId">The unique identifier of the group's owner.</param>
/// <param name="createGroup">The DTO containing data for the group creation.</param>
public class MediatR_CreateGroupCommand
(
    Guid ownerId,
    DTO_CreateGroup createGroup
) : IRequest<ObjectResult>
{
    /// <summary>
    /// The unique identifier of the group's owner.
    /// </summary>
    public Guid OwnerId { get; set; } = ownerId;
    /// <summary>
    /// Data transfer object containing information required for creating a new group.
    /// </summary>
    public DTO_CreateGroup CreateGroup { get; set; } = createGroup;
}