/* 
 * Manages group-related operations within the Group Management module. This class provides functionality 
 * for managing groups, including creating, editing, deleting, and retrieving group details and types.
 */

using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SafeShare.DataAccessLayer.Context;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.Utilities.SafeShareApi.IP;
using SafeShare.Utilities.SafeShareApi.Log;
using SafeShare.Utilities.SafeShareApi.User;
using SafeShare.ExpenseManagement.Interfaces;
using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.Utilities.SafeShareApi.Dependencies;
using SafeShare.DataAccessLayer.Models.SafeShareApi;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment;
using SafeShare.DataTransormObject.SafeShareApi.ExpenseManagment;

namespace SafeShare.GroupManagment.GroupManagment;

/// <summary>
/// Manages group operations in the Group Management module, handling tasks like creating, 
/// editing, deleting groups, and retrieving group details and types.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="GroupManagment_GroupRepository"/> class.
/// </remarks>
/// <param name="db">The application database context.</param>
/// <param name="mapper">The AutoMapper instance for object mapping.</param>
/// <param name="logger">The logger instance for logging activities.</param>
/// <param name="httpContextAccessor">The HTTP context accessor for accessing current HTTP context.</param>
public class GroupManagment_GroupRepository
(
    ApplicationDbContext db,
    IMapper mapper,
    ILogger<GroupManagment_GroupRepository> logger,
    IHttpContextAccessor httpContextAccessor,
    IGroupManagment_GroupKeyRepository groupManagment_GroupKeyRepository,
    IExpenseManagment_SecurityRepository securityExpense
) : Util_BaseContextDependencies<ApplicationDbContext, GroupManagment_GroupRepository>(
    db,
    mapper,
    logger,
    httpContextAccessor
), IGroupManagment_GroupRepository
{
    /// <summary>
    /// Retrieves the types of groups associated with a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a generic response with the group types associated with the user.</returns>
    public async Task<Util_GenericResponse<DTO_GroupsTypes>>
    GetGroupsTypes
    (
        Guid userId
    )
    {
        try
        {
            if (!await UserExists(userId))
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [GroupManagment Module]-[GroupManagment_GroupRepository class]-[GetGroupsTypes Method] => 
                        [RESULT] : [IP] {IP} user with [ID] {ID} doesnt exists.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    userId
                );

                return Util_GenericResponse<DTO_GroupsTypes>.Response
                (
                    null,
                    false,
                    $"User with id {userId} doesn't exist",
                    null,
                    System.Net.HttpStatusCode.NotFound
                );
            }

            var allGroups = await _db.GroupMembers.Include(x => x.Group)
                                      .Where(x => x.UserId == userId.ToString() && !x.Group.IsDeleted && !x.IsDeleted)
                                      .ToListAsync();

            var groupTypes = _mapper.Map<DTO_GroupsTypes>(allGroups);

            groupTypes.AllGroupsDetails = allGroups.Where(gm => !gm.IsDeleted && !gm.Group.IsDeleted)
                                        .Select(gm => new
                                        {
                                            Member = gm,
                                            GroupOwner = _db.GroupMembers
                                                            .Where(owner => owner.GroupId == gm.GroupId && owner.IsOwner)
                                                            .Select(owner => owner.User.FullName)
                                                            .FirstOrDefault(),
                                            GroupDetails = gm.Group,
                                            NumberOfMembers = _db.GroupMembers.Count(m => m.GroupId == gm.GroupId)
                                        })
                                        .ToList()
                                        .Select(x => new DTO_GroupDetails
                                        {
                                            Description = x.GroupDetails.Description,
                                            GroupAdmin = x.GroupOwner,
                                            GroupCreationDate = x.GroupDetails.CreatedAt,
                                            GroupName = x.GroupDetails.Name,
                                            NumberOfMembers = x.NumberOfMembers
                                        }).ToList();

            return Util_GenericResponse<DTO_GroupsTypes>.Response
            (
                groupTypes,
                true,
                "Group types retrieved succsessfully",
                null,
                System.Net.HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {

            return await Util_LogsHelper<DTO_GroupsTypes, GroupManagment_GroupRepository>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                    Somewthing went wrong in [GroupManagment Module]-[GroupManagment_GroupRepository class]-[GetGroupsTypes Method],
                    user with [ID] {userId} tried to get all the groups he is joined and created.
                 """,
                null,
                _httpContextAccessor
            );
        }
    }
    /// <summary>
    /// Retrieves detailed information about a specific group.
    /// </summary>
    /// <param name="userId">The unique identifier of the user requesting the group details.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a generic response with the details of the group.</returns>
    public async Task<Util_GenericResponse<DTO_GroupDetails>>
    GetGroupDetails
    (
        Guid userId,
        Guid groupId
    )
    {
        try
        {
            if (!await UserExists(userId))
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [GroupManagment Module]--[GroupManagment_GroupRepository class]--[GetGroupDetails Method] => 
                        [RESULT] : [IP] {IP} user with [ID] {ID} doesnt exists.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    userId
                );

                return Util_GenericResponse<DTO_GroupDetails>.Response
                (
                    null,
                    false,
                    $"User with id {userId} doesn't exist",
                    null,
                    System.Net.HttpStatusCode.NotFound
                );
            }

            var group = await _db.GroupMembers.Include(gr => gr.Group)
                                              .Include(usr => usr.User)
                                              .Where(gm => gm.GroupId == groupId && !gm.IsDeleted && !gm.Group.IsDeleted && gm.UserId == userId.ToString())
                                              .Select(gm => new
                                              {
                                                  Member = gm,
                                                  GroupOwner = _db.GroupMembers
                                                                 .Where(owner => owner.GroupId == gm.GroupId && owner.IsOwner)
                                                                 .Select(owner => owner.User.FullName)
                                                                 .FirstOrDefault(),
                                                  GroupDetails = gm.Group,
                                                  NumberOfMembers = _db.GroupMembers.Count(m => m.GroupId == gm.GroupId),
                                                  ImAdmin = _db.GroupMembers.Any(owner => owner.GroupId == gm.GroupId && owner.UserId == userId.ToString() && owner.IsOwner),
                                                  Members = _db.GroupMembers
                                                            .Where(x => x.GroupId == gm.GroupId && !x.IsDeleted)
                                                            .Select(x => new DTO_UsersGroupDetails
                                                            {
                                                                UserImage = x.User.ImageData,
                                                                UserName = x.User.UserName,
                                                                IsAdmin = x.IsOwner
                                                            })
                                                            .ToList()
                                              })
                                              .FirstOrDefaultAsync();

            if (group is null)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [GroupManagment Module]--[GroupManagment_GroupRepository class]--[GetGroupDetails Method] => 
                        [RESULT] : [IP] {IP} group with [ID] {ID} doesnt exists.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    groupId
                );

                return Util_GenericResponse<DTO_GroupDetails>.Response
                (
                    null,
                    false,
                    $"Group doesn't exist",
                    null,
                    System.Net.HttpStatusCode.NotFound
                );
            }

            if (String.IsNullOrEmpty(group.GroupOwner))
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [GroupManagment Module]--[GroupManagment_GroupRepository class]--[GetGroupDetails Method] => 
                        [RESULT] : [IP] {IP} group with [ID] {ID} doesnt have an admin.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    groupId
                );

                return Util_GenericResponse<DTO_GroupDetails>.Response
                (
                    null,
                    false,
                    $"Group doesn't have an admin.",
                    null,
                    System.Net.HttpStatusCode.NotFound
                );
            }

            var groupDetails = new DTO_GroupDetails
            {
                Description = group.GroupDetails.Description,
                GroupAdmin = group.GroupOwner,
                GroupCreationDate = group.GroupDetails.CreatedAt,
                GroupName = group.GroupDetails.Name,
                NumberOfMembers = group.NumberOfMembers,
                UsersGroups = group.Members,
                ImAdmin = group.ImAdmin,
            };

            return Util_GenericResponse<DTO_GroupDetails>.Response
            (
                groupDetails,
                true,
                null,
                null,
                System.Net.HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {
            return
            await Util_LogsHelper<DTO_GroupDetails, GroupManagment_GroupRepository>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                    Somewthing went wrong in [GroupManagment Module]--[GroupManagment_GroupRepository class]--[GetGroupDetails Method], 
                    user with [ID] {userId} tried to get group with [ID] {groupId} details.
                 """,
                null,
                _httpContextAccessor
            );
        }
    }
    /// <summary>
    /// Creates a new group based on provided details.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the user creating the group.</param>
    /// <param name="createGroup">The details required to create a new group.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a generic response indicating the success of the group creation.</returns>
    public async Task<Util_GenericResponse<DTO_GroupType>>
    CreateGroup
    (
        Guid ownerId,
        DTO_CreateGroup createGroup
    )
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            if (!await UserExists(ownerId))
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [GroupManagment Module]--[GroupManagment_GroupRepository class]--[CreateGroup Method] => 
                        [RESULT] : [IP] {IP} user with id {ID} doesnt exists.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    ownerId
                );

                return Util_GenericResponse<DTO_GroupType>.Response
                (
                    null,
                    false,
                    $"User with id {ownerId} doesn't exist",
                    null,
                    System.Net.HttpStatusCode.NotFound
                );
            }

            var group = _mapper.Map<Group>(createGroup);

            group.Tag = Guid.NewGuid();

            await _db.Groups.AddAsync(group);
            await _db.SaveChangesAsync();

            var groupMember = new GroupMember
            {
                IsOwner = true,
                GroupId = group.Id,
                UserId = ownerId.ToString(),
                CreatedAt = DateTime.Now,
                IsDeleted = false,
            };

            await _db.GroupMembers.AddAsync(groupMember);
            await _db.SaveChangesAsync();

            var groupType = new DTO_GroupType
            {
                GroupName = group.Name,
                GroupId = group.Id
            };

            bool result = await groupManagment_GroupKeyRepository.CreateKeyForGroup(group.Tag, group.Id);

            if (!result)
            {
                await transaction.RollbackAsync();

                return Util_GenericResponse<DTO_GroupType>.Response
                (
                    null,
                    false,
                    "Group was not created",
                    null,
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            _logger.Log
            (
                LogLevel.Information,
                """
                    [GroupManagment Module]--[GroupManagment_GroupRepository class]--[CreateGroup Method] => 
                    [RESULT] : [IP] {IP} user with id {ID} create the group with [ID] {groupId} and is a 
                    owner and a memeber of the group with.
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                ownerId,
                group.Id
            );

            await transaction.CommitAsync();

            return Util_GenericResponse<DTO_GroupType>.Response
            (
                groupType,
                true,
                "Group was created succsessfully",
                null,
                System.Net.HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            return
            await Util_LogsHelper<DTO_GroupType, GroupManagment_GroupRepository>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                        Somewthing went wrong in [GroupManagment Module]--[GroupManagment_GroupRepository class]--[CreateGroup Method], 
                        user with [ID] {ownerId} tried to create a group.
                    """,
                null,
                _httpContextAccessor
            );
        }
    }
    /// <summary>
    /// Edits the details of an existing group.
    /// </summary>
    /// <param name="userId">The unique identifier of the user editing the group.</param>
    /// <param name="groupId">The unique identifier of the group being edited.</param>
    /// <param name="editGroup">The new details for the group.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a generic response indicating the success of the group edit.</returns>
    public async Task<Util_GenericResponse<DTO_GroupType>>
    EditGroup
    (
        Guid userId,
        Guid groupId,
        DTO_EditGroup editGroup
    )
    {
        try
        {
            if (!await UserExists(userId))
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [GroupManagment Module]--[GroupManagment_GroupRepository class]--[EditGroup Method] => 
                        [RESULT] : [IP] {IP} user with [ID] {ID} doesn't exists. DTO {@DTO}.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    userId,
                    editGroup
                );

                return Util_GenericResponse<DTO_GroupType>.Response
                (
                    null,
                    false,
                    $"User with id {userId} doesn't exist",
                    null,
                    System.Net.HttpStatusCode.NotFound
                );
            }

            var isInTheGroup = await IsUserInTheGroup(userId, groupId, false, null);

            if (isInTheGroup is null)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [GroupManagment Module]--[GroupManagment_GroupRepository class]--[EditGroup Method] => 
                        [RESULT] : [IP] {IP}, group with id {groupId} 
                        created by user with id {userId} doesn't exists.
                        DTO {@DTO}
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    groupId,
                    userId,
                    editGroup
                );

                return Util_GenericResponse<DTO_GroupType>.Response
                (
                    null,
                    false,
                    $"Group with id {groupId} doesn't exists.",
                    null,
                    System.Net.HttpStatusCode.NotFound
                );
            }

            isInTheGroup.Group.ModifiedAt = DateTime.UtcNow;
            isInTheGroup.Group.Name = editGroup.GroupName;
            isInTheGroup.Group.Description = editGroup.GroupDescription;

            _db.Groups.Update(isInTheGroup.Group);
            await _db.SaveChangesAsync();

            var _group = new DTO_GroupType
            {
                GroupId = groupId,
                GroupName = editGroup.GroupName
            };

            _logger.Log
            (
                LogLevel.Information,
                """
                    [GroupManagment Module]--[GroupManagment_GroupRepository class]--[EditGroup Method] => 
                    [RESULT] : [IP] {IP}, group with id {groupId} 
                    created by user with id {userId} editted succsessfully at {groupEdittedTime}.
                    DTO {@DTO}
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                groupId,
                userId,
                isInTheGroup.Group.ModifiedAt,
                editGroup
            );

            return Util_GenericResponse<DTO_GroupType>.Response
            (
                _group,
                true,
                $"Group {editGroup.GroupName} edited succsessfully",
                null,
                System.Net.HttpStatusCode.OK
            );

        }
        catch (Exception ex)
        {
            return
            await Util_LogsHelper<DTO_GroupType, GroupManagment_GroupRepository>.ReturnInternalServerError
            (
               ex,
               _logger,
               $"""
                    Somewthing went wrong in [GroupManagment Module]--[GroupManagment_GroupRepository class]--[EditGroup Method], 
                    user with [ID] {userId} tried to edit group with [ID] {groupId}.
                """,
               null,
               _httpContextAccessor
            );
        }
    }
    /// <summary>
    /// Deletes a group.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the group's owner.</param>
    /// <param name="groupId">The unique identifier of the group being deleted.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a generic response indicating the success of the group deletion.</returns>
    public async Task<Util_GenericResponse<bool>>
    DeleteGroup
    (
        Guid ownerId,
        Guid groupId
    )
    {
        using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            if (!await UserExists(ownerId))
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [GroupManagment Module]--[GroupManagment_GroupRepository class]--[EditGroup Method] => 
                        [RESULT] : [IP] {IP},
                        user with [ID] {ownerId} doesnt exists.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    ownerId
                );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    $"User with id {ownerId} doesn't exist",
                    null,
                    System.Net.HttpStatusCode.NotFound
                );
            }

            var isInTheGroup = await IsUserInTheGroup(ownerId, groupId, false, null);

            if (isInTheGroup is null)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [GroupManagment Module]--[GroupManagment_GroupRepository class]--[DeleteGroup Method] => 
                        [RESULT] : [IP] {IP}, group with id {groupId} created by user with id {ownerId} doesn't exists.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    groupId,
                    ownerId
                );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    $"Group doesn't exists.",
                    null,
                    System.Net.HttpStatusCode.NotFound
                );
            }

            if (!isInTheGroup.IsOwner)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [GroupManagment Module]--[GroupManagment_GroupRepository class]--[DeleteGroup Method] => 
                        [RESULT] : [IP] {IP}, group with [ID] {groupId} 
                        cant be deleted from the user with [ID] {ownerId}. Not the owner of the group.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    groupId,
                    ownerId
                );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    $"Unauthorized",
                    null,
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            isInTheGroup.Group.IsDeleted = true;
            _db.Groups.Update(isInTheGroup.Group);

            var groupMembers = await _db.GroupMembers.Where(x => x.GroupId == groupId).ToListAsync();

            _db.GroupMembers.RemoveRange(groupMembers);

            var group = await _db.Groups.Include(x => x.Invitations).Include(x => x.Expenses!).ThenInclude(x => x.ExpenseMembers).FirstOrDefaultAsync(x => x.Id == groupId);

            _db.GroupInvitations.RemoveRange(group.Invitations);

            foreach (var expense in group.Expenses)
            {
                foreach (var expenseMember in expense.ExpenseMembers)
                    _db.ExpenseMembers.Remove(expenseMember);

                _db.Expenses.Remove(expense);
            }

            bool deleteGroupCryptoKey = await groupManagment_GroupKeyRepository.DeleteGroupKey(groupId);

            if (!deleteGroupCryptoKey)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [GroupManagment Module]--[GroupManagment_GroupRepository class]--[DeleteGroup Method] => 
                        [RESULT] : [IP] {IP}, group with [ID] {groupId} was not deleted due to an error.
                        """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    groupId
                );

                await transaction.RollbackAsync();
            }

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.Log
            (
                LogLevel.Information,
                """
                    [GroupManagment Module]--[GroupManagment_GroupRepository class]--[DeleteGroup Method] => 
                    [RESULT] : [IP] {IP}, group with [ID] {groupId} 
                    was deleted from the user with [ID] {ownerId} at {deleteTime}.
                    """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                groupId,
                ownerId,
                isInTheGroup.Group.DeletedAt
            );

            return Util_GenericResponse<bool>.Response
            (
                true,
                true,
                $"Group with id {groupId} was succsessfully deleted",
                null,
                System.Net.HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            return
            await Util_LogsHelper<bool, GroupManagment_GroupRepository>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                    Somewthing went wrong in [GroupManagment Module]--[GroupManagment_GroupRepository class]--[DeleteGroup Method], 
                    owner with [ID] {ownerId} tried to delete group with [ID] {groupId}.
                 """,
                false,
                _httpContextAccessor
            );
        }
    }
    /// <summary>
    /// Removes users from a group
    /// </summary>
    /// <param name="userId">The id of the owner of the group</param>
    /// <param name="groupId">The id of the group</param>
    /// <param name="usersToRemoveFromGroup">A list of members of the group</param>
    /// <returns></returns>
    public async Task<Util_GenericResponse<bool>>
    RemoveUsersFromGroup
    (
        Guid ownerId,
        Guid groupId,
        List<DTO_UsersGroupDetails> usersToRemoveFromGroup
    )
    {
        using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {

            if (!await UserExists(ownerId))
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [GroupManagment Module]--[GroupManagment_GroupRepository class]--[RemoveUsersFromGroup Method] => 
                        [RESULT] : [IP] {IP},
                        user with [ID] {ownerId} doesnt exists.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    ownerId
                );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    $"User doesn't exist",
                    null,
                    System.Net.HttpStatusCode.NotFound
                );
            }

            var isInTheGroup = await IsUserInTheGroup(ownerId, groupId, false, null);

            if (isInTheGroup is null)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [GroupManagment Module]--[GroupManagment_GroupRepository class]--[RemoveUsersFromGroup Method] => 
                        [RESULT] : [IP] {IP}, group with id {groupId} created by user with id {ownerId} doesn't exists.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    groupId,
                    ownerId
                );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    $"Group doesn't exists.",
                    null,
                    System.Net.HttpStatusCode.NotFound
                );
            }

            if (!isInTheGroup.IsOwner)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [GroupManagment Module]--[GroupManagment_GroupRepository class]--[RemoveUsersFromGroup Method] => 
                        [RESULT] : [IP] {IP},users in group with [ID] {groupId} 
                        cant be deleted from the user with [ID] {ownerId}. Not the owner of the group.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    groupId,
                    ownerId
                );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    $"Unauthorized",
                    null,
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            foreach (var item in usersToRemoveFromGroup)
            {
                var groupMember = await IsUserInTheGroup(Guid.Empty, groupId, true, item.UserName);

                if (groupMember is not null)
                    _db.GroupMembers.Remove(groupMember);
            }

            var group = await _db.Groups.Include(x => x.GroupMembers)
                                        .ThenInclude(x => x.User)
                                        .Include(x => x.Expenses!)
                                        .ThenInclude(x => x.ExpenseMembers)
                                        .FirstOrDefaultAsync(x => x.Id == groupId && !x.IsDeleted);


            CancellationTokenSource cts = new();
            CancellationToken cancellationToken = cts.Token;

            var groupMembersIds = group.GroupMembers.Where(x => x.GroupId == groupId).Select(x => x.UserId).ToList();

            var getDecryptedExpenses = await securityExpense.DecryptMultipleExpensesData(groupId, groupMembersIds, [.. group.Expenses], group.Tag, cancellationToken);

            if (!getDecryptedExpenses.Succsess || getDecryptedExpenses.Value is null)
            {
                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    "Error, user was not deleted",
                    null,
                    HttpStatusCode.BadRequest
                );
            }

            var listOfNewEncryptedExpenses = new List<Expense>();
            
            var newTag = Guid.NewGuid();

            group.Tag = newTag;
            _db.Groups.Update(group);

            bool result = await groupManagment_GroupKeyRepository.UpdateKeyForGroup(newTag, group!.Id);

            if (!result)
            {
                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    $"Error, user was not removed",
                    null,
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            foreach (var decryptedExpense in getDecryptedExpenses.Value)
            {
                var userId = decryptedExpense.ExpenseMembers.FirstOrDefault(x => x.CreatorOfExpense == true)!.UserId;

                var resultEncryption = await securityExpense.EncryptExpenseData(userId, new DTO_ExpenseCreate
                {
                    Amount = decryptedExpense.Amount,
                    Date = decryptedExpense.Date,
                    Description = decryptedExpense.Desc,
                    GroupId = decryptedExpense.GroupId,
                    Title = decryptedExpense.Title,
                }, group.Tag);

                if (!resultEncryption.Succsess || resultEncryption.Value is null)
                {
                    return Util_GenericResponse<bool>.Response
                    (
                        false,
                        false,
                        $"Error, user was not deleted",
                        null,
                        System.Net.HttpStatusCode.BadRequest
                    );
                }

                listOfNewEncryptedExpenses.Add(new Expense
                {
                    Title = resultEncryption.Value.Title,
                    Amount = resultEncryption.Value.Amount,
                    CreatedAt = decryptedExpense.CreatedAt,
                    Date = resultEncryption.Value.Date,
                    DeletedAt = decryptedExpense.DeletedAt,
                    Desc = resultEncryption.Value.Description,
                    GroupId = decryptedExpense.GroupId,
                    Id = decryptedExpense.Id,
                    IsDeleted = decryptedExpense.IsDeleted,
                    ModifiedAt = DateTime.UtcNow,
                });

                var existingExpense = await _db.Expenses.FindAsync(decryptedExpense.Id);

                if (existingExpense != null)
                    _db.Entry(existingExpense).State = EntityState.Detached;
            }

            getDecryptedExpenses.Value = null;

            _db.Expenses.UpdateRange(listOfNewEncryptedExpenses);

            await _db.SaveChangesAsync();

            await transaction.CommitAsync();

            _logger.Log
            (
               LogLevel.Information,
               """
                    [GroupManagment Module]--[GroupManagment_GroupRepository class]--[RemoveUsersFromGroup Method] => 
                    [RESULT] : [IP] {IP},users {@users} in group with [ID] {groupId} 
                    were deleted from the user with [ID] {ownerId} at {deleteTime}.
                """,
               await Util_GetIpAddres.GetLocation(_httpContextAccessor),
               usersToRemoveFromGroup,
               groupId,
               ownerId,
               isInTheGroup.Group.DeletedAt
            );

            return Util_GenericResponse<bool>.Response
            (
                true,
                true,
                $"Users were succsessfully removed from the group",
                null,
                System.Net.HttpStatusCode.OK
            );

        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            return
            await Util_LogsHelper<bool, GroupManagment_GroupRepository>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                    Somewthing went wrong in [GroupManagment Module]--[GroupManagment_GroupRepository class]--[RemoveUsersFromGroup Method], 
                    owner with [ID] {ownerId} tried to delete group with [ID] {groupId}.
                 """,
                false,
                _httpContextAccessor
            );
        }
    }

    /// <summary>
    /// Checks if a user exists in the database.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to check.</param>
    /// <returns>A task representing the asynchronous operation. The task result is a boolean indicating whether the user exists.</returns>
    private async Task<bool>
    UserExists
    (
        Guid userId
    )
    {
        return await _db.Users.AnyAsync(x => x.Id == userId.ToString() && !x.IsDeleted);
    }
    /// <summary>
    /// Checks if a user is a member of a specified group.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <returns>A task representing the asynchronous operation. The task result is a GroupMember object if the user is in the group; otherwise, null.</returns>
    private async Task<GroupMember?>
    IsUserInTheGroup
    (
        Guid userId,
        Guid groupId,
        bool byUsername,
        string? username
    )
    {
        if (byUsername && username != null)
        {
            return await _db.GroupMembers.Include(x => x.Group)
                                    .FirstOrDefaultAsync
                                    (
                                       x => x.GroupId == groupId &&
                                       x.User.UserName == username &&
                                       !x.Group.IsDeleted &&
                                       !x.IsDeleted
                                    );
        }

        return await _db.GroupMembers.Include(x => x.Group)
                                     .FirstOrDefaultAsync
                                     (
                                        x => x.GroupId == groupId &&
                                        x.UserId == userId.ToString() &&
                                        !x.Group.IsDeleted &&
                                        !x.IsDeleted
                                     );
    }
}