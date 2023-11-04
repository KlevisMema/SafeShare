using AutoMapper;
using SafeShare.Utilities.Log;
using SafeShare.Utilities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SafeShare.Utilities.Services;
using Microsoft.EntityFrameworkCore;
using SafeShare.Utilities.Responses;
using Microsoft.AspNetCore.Identity;
using SafeShare.Utilities.Dependencies;
using SafeShare.DataAccessLayer.Models;
using SafeShare.DataAccessLayer.Context;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.DataTransormObject.GroupManagment.GroupInvitations;

namespace SafeShare.GroupManagment.GroupManagment;

public class GroupManagment_GroupInvitationsRepository :
    Util_BaseContextDependencies<ApplicationDbContext, GroupManagment_GroupInvitationsRepository>,
    IGroupManagment_GroupInvitationsRepository
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

    public async Task<Util_GenericResponse<List<DTO_RecivedInvitations>>>
    GetRecivedGroupsInvitations
    (
        Guid userId
    )
    {
        try
        {
            var invitations = await _db.GroupInvitations.Include(x => x.Group)
                                                        .Include(x => x.InvitingUser)
                                                        .Include(x => x.InvitedUser)
                                                        .Where(x => x.InvitedUserId == userId.ToString() && !x.IsDeleted)
                                                        .ToListAsync();

            return Util_GenericResponse<List<DTO_RecivedInvitations>>.Response
            (
                _mapper.Map<List<DTO_RecivedInvitations>>(invitations),
                true,
                "Invitations retrieved succsessfully",
                null,
                System.Net.HttpStatusCode.OK
            );
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<Util_GenericResponse<List<DTO_SentInvitations>>>
    GetSentGroupInvitations
    (
       Guid userId
    )
    {
        try
        {
            var sentGroupInvitations = await _db.GroupInvitations.Include(x => x.Group)
                                                                 .Include(x => x.InvitedUser)
                                                                 .Where
                                                                 (
                                                                    inv => inv.InvitingUserId == userId.ToString() &&
                                                                    !inv.IsDeleted
                                                                 )
                                                                 .ToListAsync();

            return Util_GenericResponse<List<DTO_SentInvitations>>.Response
            (
                _mapper.Map<List<DTO_SentInvitations>>(sentGroupInvitations),
                true,
                "Sent invitations recived succsessfully",
                null,
                System.Net.HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public async Task<Util_GenericResponse<bool>>
    SendInvitation
    (
        DTO_SendInvitationRequest sendInvitation
    )
    {
        try
        {
            var invitation = new GroupInvitation
            {
                InvitingUserId = sendInvitation.InvitingUserId.ToString(),
                InvitedUserId = sendInvitation.InvitedUserId.ToString(),
                GroupId = sendInvitation.GroupId,
                InvitationStatus = InvitationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                InvitationMessage = sendInvitation.InvitaitonMessage
            };

            await _db.GroupInvitations.AddAsync(invitation);
            await _db.SaveChangesAsync();

            return Util_GenericResponse<bool>.Response
            (
                true,
                true,
                "Invitation sent succsessfully",
                null,
                System.Net.HttpStatusCode.OK
            );

        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<bool, GroupManagment_GroupInvitationsRepository>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"Somewthing went wrong in [GroupManagment Module] - [SendInvitation Method], " +
                $"user with [ID] {sendInvitation.InvitingUserId} tried to send an invite to user with " +
                $"[ID] {sendInvitation.InvitedUserId} to the group with [ID] {sendInvitation.GroupId}",
                false,
                _httpContextAccessor
            );
        }
    }

    public async Task<Util_GenericResponse<bool>>
    AcceptInvitation
    (
        DTO_InvitationRequestActions accepInvitation
    )
    {
        try
        {
            var invitation = await _db.GroupInvitations.FirstOrDefaultAsync(x => x.Id == accepInvitation.InvitationId);

            if
            (
                invitation != null &&
                !invitation.IsDeleted &&
                invitation.InvitingUserId == accepInvitation.InvitingUserId.ToString() &&
                invitation.GroupId == accepInvitation.GroupId &&
                invitation.InvitedUserId == accepInvitation.InvitedUserId.ToString() &&
                invitation.InvitationStatus == InvitationStatus.Pending
            )
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

                return Util_GenericResponse<bool>.Response
                (
                    true,
                    true,
                    "Invitation accepted succsessfully",
                    null,
                    System.Net.HttpStatusCode.OK
                );
            }

            return Util_GenericResponse<bool>.Response
            (
                false,
                false,
                "Invitation doesn't exists",
                null,
                System.Net.HttpStatusCode.NotFound
            );
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public async Task<Util_GenericResponse<bool>>
    RejectInvitation
    (
        DTO_InvitationRequestActions rejectInvitation
    )
    {
        try
        {
            var invitation = await _db.GroupInvitations.FirstOrDefaultAsync(x => x.Id == rejectInvitation.InvitationId);

            if
            (
                invitation != null &&
                !invitation.IsDeleted &&
                invitation.InvitingUserId == rejectInvitation.InvitingUserId.ToString() &&
                invitation.GroupId == rejectInvitation.GroupId &&
                invitation.InvitedUserId == rejectInvitation.InvitedUserId.ToString() &&
                invitation.InvitationStatus == InvitationStatus.Pending
            )
            {
                invitation.InvitationStatus = InvitationStatus.Rejected;
                invitation.ModifiedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();

                return Util_GenericResponse<bool>.Response
                (
                   true,
                   true,
                   "Invitation rejected succsessfully",
                   null,
                   System.Net.HttpStatusCode.OK
                );
            }

            return Util_GenericResponse<bool>.Response
            (
               false,
               false,
               "Something went wrong",
               null,
               System.Net.HttpStatusCode.NotFound
            );

        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public async Task<Util_GenericResponse<bool>>
    DeleteSentInvitation
    (
        DTO_InvitationRequestActions deleteInvitation
    )
    {
        try
        {
            var invitation = await _db.GroupInvitations.FirstOrDefaultAsync(x => x.Id == deleteInvitation.InvitationId);

            if
            (
                invitation != null &&
                !invitation.IsDeleted &&
                invitation.InvitingUserId == deleteInvitation.InvitingUserId.ToString() &&
                invitation.GroupId == deleteInvitation.GroupId &&
                invitation.InvitedUserId == deleteInvitation.InvitedUserId.ToString() &&
                invitation.InvitationStatus == InvitationStatus.Pending || invitation.InvitationStatus == InvitationStatus.Rejected
            )
            {
                invitation.IsDeleted = true;
                invitation.DeletedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();

                return Util_GenericResponse<bool>.Response
                (
                    true,
                    true,
                    "Invitation deleted succsessfully",
                    null,
                    System.Net.HttpStatusCode.OK
                );
            }

            return Util_GenericResponse<bool>.Response
            (
                false,
                false,
                "Something went wrong",
                null,
                System.Net.HttpStatusCode.NotFound
            );



        }
        catch (Exception)
        {

            throw;
        }
    }
}