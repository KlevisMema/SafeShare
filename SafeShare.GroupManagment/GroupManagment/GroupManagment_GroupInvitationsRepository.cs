using AutoMapper;
using SafeShare.Utilities.Log;
using SafeShare.Utilities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SafeShare.Utilities.Services;
using Microsoft.EntityFrameworkCore;
using SafeShare.Utilities.Responses;
using Microsoft.AspNetCore.Identity;
using SafeShare.DataAccessLayer.Context;
using SafeShare.DataAccessLayer.Models;
using SafeShare.Utilities.Dependencies;
using SafeShare.GroupManagment.Interfaces;

namespace SafeShare.GroupManagment.GroupManagment;

public class GroupManagment_GroupInvitationsRepository : Util_BaseContextDependencies<ApplicationDbContext, GroupManagment_GroupInvitationsRepository>, IGroupManagment_GroupInvitationsRepository
{
    public GroupManagment_GroupInvitationsRepository
    (
        ApplicationDbContext db,
        IMapper mapper,
        ILogger<GroupManagment_GroupInvitationsRepository> logger,
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

    public async Task<Util_GenericResponse<bool>>
    SendInvitation
    (
        string invitingUserId,
        string invitedUserId,
        Guid groupId
    )
    {
        try
        {
            var invitation = new GroupInvitation
            {
                InvitingUserId = invitingUserId,
                InvitedUserId = invitedUserId,
                GroupId = groupId,
                InvitationStatus = InvitationStatus.Pending
            };

            await _db.GroupInvitations.AddAsync(invitation);
            _db.SaveChanges();

            return Util_GenericResponse<bool>.Response(true, true, "Invitation sent succsessfully", null, System.Net.HttpStatusCode.OK);

        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<bool, GroupManagment_GroupInvitationsRepository>.ReturnInternalServerError(ex, _logger, $"Somewthing went wrong in [GroupManagment Module] - [SendInvitation Method], user with [ID] {invitingUserId}", false, _httpContextAccessor);
        }
    }

    public async Task<Util_GenericResponse<bool>>
    AcceptInvitation
    (
        Guid invitationId
    )
    {
        try
        {
            var invitation = _db.GroupInvitations.Find(invitationId);

            if (invitation != null && invitation.InvitationStatus == InvitationStatus.Pending)
            {
                invitation.InvitationStatus = InvitationStatus.Accepted;
                await _db.SaveChangesAsync();

                var groupMember = new GroupMember
                {
                    CreatedAt = DateTime.Now,
                    IsOwner = false,
                    IsDeleted = false,
                    GroupId = invitation.GroupId,
                    UserId = invitation.InvitedUserId,
                };

                await _db.GroupMembers.AddAsync(groupMember);
                await _db.SaveChangesAsync();

                return Util_GenericResponse<bool>.Response(true, true, "Invitation accepted succsessfully", null, System.Net.HttpStatusCode.OK);
            }

            return Util_GenericResponse<bool>.Response(false, false, "Invitation doesn't exists", null, System.Net.HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public async void
    RejectInvitation
    (
        int invitationId
    )
    {
        try
        {
            var invitation = _db.GroupInvitations.Find(invitationId);

            if (invitation != null && invitation.InvitationStatus == InvitationStatus.Pending)
            {
                invitation.InvitationStatus = InvitationStatus.Rejected;
                await _db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public List<GroupInvitation>
    GetSentInvitations
    (
        string userId
    )
    {
        try
        {
            return _db.GroupInvitations
              .Where(inv => inv.InvitingUserId == userId)
          .ToList();
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public List<GroupInvitation>
    GetReceivedInvitations
    (
        string userId
    )
    {
        try
        {
            return _db.GroupInvitations
               .Where(inv => inv.InvitedUserId == userId)
               .ToList();
        }
        catch (Exception ex)
        {

            throw;
        }
    }
}