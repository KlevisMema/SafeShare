/* 
 * Defines the contract for a service responsible for group-related operations within the Group Management module. 
 * This interface provides functionality for managing groups, including creating, editing, deleting, and retrieving group details and types.
 */

using SafeShare.DataTransormObject.SafeShareApi.GroupManagment;
using SafeShare.Utilities.SafeShareApi.Responses;

namespace SafeShare.GroupManagment.Interfaces;

/// <summary>
/// Defines the contract for a repository managing group operations in the Group Management module. 
/// Includes methods for creating, editing, deleting groups, and retrieving group details and types.
/// </summary>
public interface IGroupManagment_GroupRepository
{
    /// <summary>
    /// Retrieves the types of groups associated with a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a generic response with the group types associated with the user.</returns>
    Task<Util_GenericResponse<DTO_GroupsTypes>>
    GetGroupsTypes
    (
        Guid userId
    );
    /// <summary>
    /// Retrieves detailed information about a specific group.
    /// </summary>
    /// <param name="userId">The unique identifier of the user requesting the group details.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a generic response with the details of the group.</returns>
    Task<Util_GenericResponse<DTO_GroupDetails>>
    GetGroupDetails
    (
        Guid userId,
        Guid groupId
    );
    /// <summary>
    /// Creates a new group based on provided details.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the user creating the group.</param>
    /// <param name="createGroup">The details required to create a new group.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a generic response indicating the success of the group creation.</returns>
    Task<Util_GenericResponse<DTO_GroupType>>
    CreateGroup
    (
        Guid ownerId,
        DTO_CreateGroup createGroup
    );
    /// <summary>
    /// Edits the details of an existing group.
    /// </summary>
    /// <param name="userId">The unique identifier of the user editing the group.</param>
    /// <param name="groupId">The unique identifier of the group being edited.</param>
    /// <param name="editGroup">The new details for the group.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a generic response indicating the success of the group edit.</returns>
    Task<Util_GenericResponse<DTO_GroupType>>
    EditGroup
    (
        Guid userId,
        Guid groupId,
        DTO_EditGroup editGroup
    );
    /// <summary>
    /// Deletes a group.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the group's owner.</param>
    /// <param name="groupId">The unique identifier of the group being deleted.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a generic response indicating the success of the group deletion.</returns>
    Task<Util_GenericResponse<bool>>
    DeleteGroup
    (
        Guid ownerId,
        Guid groupId
    );
    /// <summary>
    /// Removes users from a group
    /// </summary>
    /// <param name="userId">The id of the owner of the group</param>
    /// <param name="groupId">The id of the group</param>
    /// <param name="usersToRemoveFromGroup">A list of members of the group</param>
    /// <returns></returns>
    Task<Util_GenericResponse<bool>>
    RemoveUsersFromGroup
    (
        Guid ownerId,
        Guid groupId,
        List<DTO_UsersGroupDetails> usersToRemoveFromGroup
    );
}