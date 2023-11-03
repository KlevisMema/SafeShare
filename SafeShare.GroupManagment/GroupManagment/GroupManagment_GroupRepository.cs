using AutoMapper;
using SafeShare.Utilities.IP;
using SafeShare.Utilities.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SafeShare.Utilities.Services;
using SafeShare.Utilities.Responses;
using Microsoft.EntityFrameworkCore;
using SafeShare.DataAccessLayer.Models;
using SafeShare.Utilities.Dependencies;
using SafeShare.DataAccessLayer.Context;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.DataTransormObject.GroupManagment;

namespace SafeShare.GroupManagment.GroupManagment;

public class GroupManagment_GroupRepository : Util_BaseContextDependencies<ApplicationDbContext, GroupManagment_GroupRepository>, IGroupManagment_GroupRepository
{
    public GroupManagment_GroupRepository
    (
        ApplicationDbContext db,
        IMapper mapper,
        ILogger<GroupManagment_GroupRepository> logger,
        IHttpContextAccessor httpContextAccessor
    )
    : base
    (
        db,
        mapper,
        logger,
        httpContextAccessor
    )
    { }

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
                _logger.Log(LogLevel.Information, $"[GroupManagment Module]-[GroupManagment_GroupRepository class]-[GetGroupsTypes Method] => [RESULT] : [IP] {await Util_GetIpAddres.GetLocation(_httpContextAccessor)} user with id {userId} doesnt exists");
                return Util_GenericResponse<DTO_GroupsTypes>.Response(null, false, $"User with id {userId} doesn't exist", null, System.Net.HttpStatusCode.NotFound);
            }

            var allGroups = await _db.GroupMembers.Include(x => x.Group)
                                      .Where(x => x.UserId == userId.ToString() && !x.Group.IsDeleted && !x.IsDeleted)
                                      .ToListAsync();

            var groupTypes = _mapper.Map<DTO_GroupsTypes>(allGroups);

            return Util_GenericResponse<DTO_GroupsTypes>.Response(groupTypes, true, "Group types retrieved succsessfully", null, System.Net.HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return
            await Util_LogsHelper<DTO_GroupsTypes, GroupManagment_GroupRepository>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"Somewthing went wrong in [GroupManagment Module]-[GroupManagment_GroupRepository class]-[GetGroupsTypes Method], user with [ID] {userId} tried to get all the groups he is joined and created.",
                null,
                _httpContextAccessor
            );
        }
    }

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
                _logger.Log(LogLevel.Information, $"[GroupManagment Module]--[GroupManagment_GroupRepository class]--[GetGroupDetails Method] => [RESULT] : [IP] {await Util_GetIpAddres.GetLocation(_httpContextAccessor)} user with id {userId} doesnt exists");
                return Util_GenericResponse<DTO_GroupDetails>.Response(null, false, $"User with id {userId} doesn't exist", null, System.Net.HttpStatusCode.NotFound);
            }

            var group = await _db.GroupMembers.Include(gr => gr.Group)
                                              .Where(gm => gm.GroupId == groupId && !gm.IsDeleted && !gm.Group.IsDeleted)
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
                                              .FirstOrDefaultAsync(gm => gm.Member.UserId == userId.ToString());

            if (group is null)
            {
                _logger.Log(LogLevel.Information, $"[GroupManagment Module]--[GroupManagment_GroupRepository class]--[GetGroupDetails Method] => [RESULT] : [IP] {await Util_GetIpAddres.GetLocation(_httpContextAccessor)} group with id {groupId} doesnt exists");
                return Util_GenericResponse<DTO_GroupDetails>.Response(null, false, $"Group with id {groupId} doesn't exist", null, System.Net.HttpStatusCode.NotFound);
            }

            if (String.IsNullOrEmpty(group.GroupOwner))
            {
                _logger.Log(LogLevel.Information, $"[GroupManagment Module]--[GroupManagment_GroupRepository class]--[GetGroupDetails Method] => [RESULT] : [IP] {await Util_GetIpAddres.GetLocation(_httpContextAccessor)} group with id {groupId} doesnt have an admin.");
                return Util_GenericResponse<DTO_GroupDetails>.Response(null, false, $"Group with id {groupId} doesn't have an admin.", null, System.Net.HttpStatusCode.NotFound);
            }

            var groupDetails = new DTO_GroupDetails
            {
                Description = group.GroupDetails.Description,
                GroupAdmin = group.GroupOwner,
                GroupCreationDate = group.GroupDetails.CreatedAt,
                GroupName = group.GroupDetails.Name,
                LatestExpense = "",
                TotalSpent = group.Member.Balance,
                NumberOfMembers = group.NumberOfMembers
            };

            return Util_GenericResponse<DTO_GroupDetails>.Response(groupDetails, true, null, null, System.Net.HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return
            await Util_LogsHelper<DTO_GroupDetails, GroupManagment_GroupRepository>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"Somewthing went wrong in [GroupManagment Module]--[GroupManagment_GroupRepository class]--[GetGroupDetails Method], user with [ID] {userId} tried to get group with [ID] {groupId} details.",
                null,
                _httpContextAccessor
            );
        }
    }

    public async Task<Util_GenericResponse<DTO_GroupType>>
    CreateGroup
    (
        Guid ownerId,
        DTO_CreateGroup createGroup
    )
    {
        try
        {
            if (!await UserExists(ownerId))
            {
                _logger.Log(LogLevel.Information, $"[GroupManagment Module]--[GroupManagment_GroupRepository class]--[CreateGroup Method] => [RESULT] : [IP] {await Util_GetIpAddres.GetLocation(_httpContextAccessor)} user with id {ownerId} doesnt exists");
                return Util_GenericResponse<DTO_GroupType>.Response(null, false, $"User with id {ownerId} doesn't exist", null, System.Net.HttpStatusCode.NotFound);
            }

            var group = _mapper.Map<Group>(createGroup);

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

            return Util_GenericResponse<DTO_GroupType>.Response(groupType, true, "Group was created succsessfully", null, System.Net.HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return
            await Util_LogsHelper<DTO_GroupType, GroupManagment_GroupRepository>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"Somewthing went wrong in [GroupManagment Module]--[GroupManagment_GroupRepository class]--[CreateGroup Method], user with [ID] {ownerId} tried to create a group.",
                null,
                _httpContextAccessor
            );
        }
    }

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
                _logger.Log(LogLevel.Information, $"[GroupManagment Module]--[GroupManagment_GroupRepository class]--[EditGroup Method] => [RESULT] : [IP] {await Util_GetIpAddres.GetLocation(_httpContextAccessor)} user with id {userId} doesnt exists");
                return Util_GenericResponse<DTO_GroupType>.Response(null, false, $"User with id {userId} doesn't exist", null, System.Net.HttpStatusCode.NotFound);
            }

            var isInTheGroup = await IsUserInTheGroup(userId, groupId);

            if (isInTheGroup is null)
            {
                _logger.Log(LogLevel.Information, $"[GroupManagment Module]--[GroupManagment_GroupRepository class]--[EditGroup Method] => [RESULT] : [IP] {await Util_GetIpAddres.GetLocation(_httpContextAccessor)} group with id {groupId} created by user with id {userId} doesn't exists.");
                return Util_GenericResponse<DTO_GroupType>.Response(null, false, $"Group with id {groupId} doesn't exists.", null, System.Net.HttpStatusCode.NotFound);
            }

            var group = new Group
            {
                Id = groupId,
                ModifiedAt = DateTime.Now,
                Name = editGroup.GroupName,
                Description = editGroup.GroupDescription
            };

            _db.Groups.Update(group);
            await _db.SaveChangesAsync();

            var _group = new DTO_GroupType
            {
                GroupId = groupId,
                GroupName = editGroup.GroupName
            };

            return Util_GenericResponse<DTO_GroupType>.Response(_group, true, $"Group {editGroup.GroupName} edited succsessfully", null, System.Net.HttpStatusCode.OK);

        }
        catch (Exception ex)
        {
            return
            await Util_LogsHelper<DTO_GroupType, GroupManagment_GroupRepository>.ReturnInternalServerError
            (
               ex,
               _logger,
               $"Somewthing went wrong in [GroupManagment Module]--[GroupManagment_GroupRepository class]--[EditGroup Method], user with [ID] {userId} tried to edit group with [ID] {groupId}.",
               null,
               _httpContextAccessor
            );
        }
    }

    public async Task<Util_GenericResponse<bool>>
    DeleteGroup
    (
        Guid ownerId,
        Guid groupId
    )
    {
        try
        {
            if (!await UserExists(ownerId))
            {
                _logger.Log(LogLevel.Information, $"[GroupManagment Module]--[GroupManagment_GroupRepository class]--[EditGroup Method] => [RESULT] : [IP] {await Util_GetIpAddres.GetLocation(_httpContextAccessor)} user with id {ownerId} doesnt exists");
                return Util_GenericResponse<bool>.Response(false, false, $"User with id {ownerId} doesn't exist", null, System.Net.HttpStatusCode.NotFound);
            }

            var isInTheGroup = await IsUserInTheGroup(ownerId, groupId);

            if (isInTheGroup is null)
            {
                _logger.Log(LogLevel.Information, $"[GroupManagment Module]--[GroupManagment_GroupRepository class]--[DeleteGroup Method] => [RESULT] : [IP] {await Util_GetIpAddres.GetLocation(_httpContextAccessor)} group with id {groupId} created by user with id {ownerId} doesn't exists.");
                return Util_GenericResponse<bool>.Response(false, false, $"Group with id {groupId} doesn't exists.", null, System.Net.HttpStatusCode.NotFound);
            }

            if (!isInTheGroup.IsOwner)
            {
                _logger.Log(LogLevel.Information, $"[GroupManagment Module]--[GroupManagment_GroupRepository class]--[DeleteGroup Method] => [RESULT] : [IP] {await Util_GetIpAddres.GetLocation(_httpContextAccessor)} group with id {groupId} cant be deleted from the user with [ID] {ownerId}. Not the owner of the group.");
                return Util_GenericResponse<bool>.Response(false, false, $"Group with id {groupId} can't be deleted because you are not the owner.", null, System.Net.HttpStatusCode.BadRequest);
            }

            isInTheGroup.Group.IsDeleted = true;
            _db.Groups.Update(isInTheGroup.Group);
            await _db.SaveChangesAsync();

            var groupMembers = await _db.GroupMembers.Where(x => x.GroupId == groupId).ToListAsync();

            groupMembers.ForEach(x => x.IsDeleted = true);

            _db.GroupMembers.UpdateRange(groupMembers);
            await _db.SaveChangesAsync();

            return Util_GenericResponse<bool>.Response(true, true, $"Group with id {groupId} was succsessfully deleted", null, System.Net.HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return
            await Util_LogsHelper<bool, GroupManagment_GroupRepository>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"Somewthing went wrong in [GroupManagment Module]--[GroupManagment_GroupRepository class]--[DeleteGroup Method], owner with [ID] {ownerId} tried to delete group with [ID] {groupId}.",
                false,
                _httpContextAccessor
            );
        }
    }

    private async Task<bool>
    UserExists
    (
        Guid userId
    )
    {
        return await _db.Users.AnyAsync(x => x.Id == userId.ToString() && !x.IsDeleted);
    }

    private async Task<GroupMember?>
    IsUserInTheGroup
    (
        Guid userId,
        Guid groupId
    )
    {
        return await _db.GroupMembers.Include(x => x.Group).FirstOrDefaultAsync(x => x.GroupId == groupId && x.UserId == userId.ToString() && !x.Group.IsDeleted && !x.IsDeleted);
    }
}