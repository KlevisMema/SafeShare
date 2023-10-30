using AutoMapper;
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
            var groupsCreated = await _db.GroupMembers.Include(x => x.Group)
                                                      .Where(x => x.UserId == userId.ToString() && x.IsOwner)
                                                      .ToListAsync();

            var groupsJoined = await _db.GroupMembers.Include(x => x.Group)
                                                     .Where(x => x.UserId == userId.ToString() && !x.IsOwner)
                                                     .ToListAsync();

            var test = new DTO_GroupsTypes
            {
                GroupsCreated = groupsCreated.Select(x => new DTO_GroupType
                {
                    GroupId = x.GroupId,
                    GroupName = x.Group.Name,
                }).ToList(),

                GroupsJoined = groupsJoined.Select(x => new DTO_GroupType
                {
                    GroupId = x.GroupId,
                    GroupName = x.Group.Name
                }).ToList()
            };

            return Util_GenericResponse<DTO_GroupsTypes>.Response(test, true, null, null, System.Net.HttpStatusCode.OK);
        }
        catch (Exception)
        {

            throw;
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
            var group = await _db.GroupMembers.Include(x => x.User)
                                              .Include(x => x.Group)
                                              .FirstOrDefaultAsync(x => x.UserId == userId.ToString() && x.GroupId == groupId);

            var groupAdmin = await _db.GroupMembers.Include(x => x.User)
                                                   .Include(x => x.Group)
                                                   .Where(x => x.GroupId == groupId && x.IsOwner)
                                                   .FirstOrDefaultAsync();

            var numberOfMembers = await _db.GroupMembers.Include(x => x.User)
                                                   .Include(x => x.Group)
                                                   .Where(x => x.GroupId == groupId).CountAsync();

            var test = new DTO_GroupDetails
            {
                Description = group.Group.Description,
                GroupAdmin = groupAdmin.User.FullName,
                GroupCreationDate = group.CreatedAt,
                GroupName = group.Group.Name,
                LatestExpense = "$50 for dinner at XYZ Restaurant on 29th October",
                TotalSpent = group.Balance,
                NumberOfMembers = numberOfMembers
            };

            return Util_GenericResponse<DTO_GroupDetails>.Response(test, true, null, null, System.Net.HttpStatusCode.OK);

        }
        catch (Exception ex)
        {

            throw;
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

            throw;
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
            var isInTheGroup = await _db.GroupMembers.FirstOrDefaultAsync(x => x.GroupId == groupId && x.UserId == userId.ToString());

            if (isInTheGroup is null)
            {
                // do smth if user is not part of the group
            }

            var group = new Group
            {
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

            throw;
        }
    }
}