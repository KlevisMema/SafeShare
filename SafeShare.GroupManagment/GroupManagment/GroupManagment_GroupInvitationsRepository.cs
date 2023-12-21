/* 
 * Manages group invitation operations within the Group Management module. It provides functionalities 
 * like receiving, sending, accepting, and rejecting group invitations.
 */

using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using SafeShare.DataAccessLayer.Models;
using SafeShare.DataAccessLayer.Context;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.Utilities.SafeShareApi.IP;
using SafeShare.Utilities.SafeShareApi.Log;
using SafeShare.Utilities.SafeShareApi.Enums;
using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.DataAccessLayer.Models.SafeShareApi;
using SafeShare.Utilities.SafeShareApi.Dependencies;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment.GroupInvitations;

namespace SafeShare.GroupManagment.GroupManagment;

/// <summary>
/// Represents the repository for managing group invitations in the Group Management module. 
/// It includes functionalities for handling various aspects of group invitations such as receiving, 
/// sending, accepting, and rejecting them.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="GroupManagment_GroupInvitationsRepository"/> class.
/// </remarks>
/// <param name="db">The database context used for data operations.</param>
/// <param name="mapper">The AutoMapper instance for object mapping.</param>
/// <param name="logger">The logger for logging information and errors.</param>
/// <param name="httpContextAccessor">Provides access to the current HTTP context.</param>
public class GroupManagment_GroupInvitationsRepository
(
    ApplicationDbContext db,
    IMapper mapper,
    ILogger<GroupManagment_GroupInvitationsRepository> logger,
    IHttpContextAccessor httpContextAccessor
) : Util_BaseContextDependencies<ApplicationDbContext, GroupManagment_GroupInvitationsRepository>(
    db,
    mapper,
    logger,
    httpContextAccessor
), IGroupManagment_GroupInvitationsRepository
{
    /// <summary>
    /// Retrieves a list of received group invitations for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose received invitations are to be fetched.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a generic response with a list of received invitations.</returns>
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
        catch (Exception ex)
        {
            return await Util_LogsHelper<List<DTO_RecivedInvitations>, GroupManagment_GroupInvitationsRepository>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                Somewthing went wrong in [GroupManagment Module]-[GroupManagment_GroupInvitationsRepository class]-[GetRecivedGroupsInvitations Method],
                user with [ID] {userId} tried to get all the recived invitations.
                """,
                null,
                _httpContextAccessor
            );
        }
    }
    /// <summary>
    /// Retrieves a list of sent group invitations for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose sent invitations are to be fetched.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a generic response with a list of sent invitations.</returns>
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
            return await Util_LogsHelper<List<DTO_SentInvitations>, GroupManagment_GroupInvitationsRepository>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                Somewthing went wrong in [GroupManagment Module]-[GroupManagment_GroupInvitationsRepository class]-[GetSentGroupInvitations Method],
                user with [ID] {userId} tried to get all the recived invitations.
                """,
                null,
                _httpContextAccessor
            );
        }
    }
    /// <summary>
    /// Sends a group invitation from one user to another.
    /// </summary>
    /// <param name="sendInvitation">The details of the invitation being sent.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a generic response indicating the success or failure of sending the invitation.</returns>
    public async Task<Util_GenericResponse<bool>>
    SendInvitation
    (
        DTO_SendInvitationRequest sendInvitation
    )
    {
        try
        {
            var checkPassed = await GenerealChecks("SendInvitation", sendInvitation.GroupId, sendInvitation.InvitingUserId, sendInvitation.InvitedUserId);

            if (!checkPassed)
            {
                return Util_GenericResponse<bool>.Response
                (
                   false,
                   false,
                   "Something went wrong!",
                   null,
                   System.Net.HttpStatusCode.BadRequest
                );
            }

            var isAdminWhoInvited = await _db.GroupMembers.AnyAsync
            (
                x => x.GroupId == sendInvitation.GroupId &&
                x.UserId == sendInvitation.InvitingUserId.ToString()
                && x.IsOwner
            );

            if (!isAdminWhoInvited)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [GroupManagment Module]--[GroupManagment_GroupInvitationsRepository class]--[SendInvitation Method] => 
                        [RESULT] : [IP] {IP} user with [ID] {ID} invited user with id {InvitedUserId} the group with id {groupId}.
                        But he is not he admin of the group!
                        More details : {@invitationDto}
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    sendInvitation.InvitingUserId,
                    sendInvitation.InvitedUserId,
                    sendInvitation.GroupId,
                    sendInvitation
                );

                return Util_GenericResponse<bool>.Response
                (
                   false,
                   false,
                   "Something went wrong!",
                   null,
                   System.Net.HttpStatusCode.Unauthorized
                );
            }

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

            _logger.Log
            (
                LogLevel.Information,
                """
                    [GroupManagment Module]--[GroupManagment_GroupInvitationsRepository class]--[SendInvitation Method] => 
                    [RESULT] : [IP] {IP} user with [ID] {ID} invited user with id {InvitedUserId} the group with id {groupId}.
                    More details : {@invitationDto}
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                sendInvitation.InvitingUserId,
                sendInvitation.InvitedUserId,
                sendInvitation.GroupId,
                sendInvitation
            );

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
                $"""
                    Somewthing went wrong in [GroupManagment Module] - [SendInvitation Method],
                    user with [ID] {sendInvitation.InvitingUserId} tried to send an invite to user with
                    [ID] {sendInvitation.InvitedUserId} to the group with [ID] {sendInvitation.GroupId}.
                 """,
                false,
                _httpContextAccessor
            );
        }
    }
    /// <summary>
    /// Accepts a received group invitation.
    /// </summary>
    /// <param name="accepInvitation">The details of the invitation being accepted.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a generic response indicating the success or failure of accepting the invitation.</returns>
    public async Task<Util_GenericResponse<bool>>
    AcceptInvitation
    (
        DTO_InvitationRequestActions accepInvitation
    )
    {
        try
        {
            var checkPassed = await GenerealChecks("AcceptInvitation", accepInvitation.GroupId, accepInvitation.InvitingUserId, accepInvitation.InvitedUserId);

            if (!checkPassed)
            {
                return Util_GenericResponse<bool>.Response
                (
                   false,
                   false,
                   "Something went wrong!",
                   null,
                   System.Net.HttpStatusCode.BadRequest
                );
            }

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

                _logger.Log
                (
                    LogLevel.Information,
                    """
                        [GroupManagment Module]-[GroupManagment_GroupInvitationsRepository class]-[AcceptInvitation Method]
                        Request with IP {IP}.
                        User with id {invitedUserId} accepted the invitation with id {invitationId} made by 
                        the user with id {invitingUserId} for the group with id {groupId} but the invitation with that 
                        id doesn't exists. Dto {@DTO}
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    accepInvitation.InvitedUserId,
                    accepInvitation.InvitationId,
                    accepInvitation.InvitingUserId,
                    accepInvitation.GroupId,
                    accepInvitation
                );

                return Util_GenericResponse<bool>.Response
                (
                    true,
                    true,
                    "Invitation accepted succsessfully",
                    null,
                    System.Net.HttpStatusCode.OK
                );
            }

            _logger.Log
            (
                LogLevel.Error,
                """
                     Request with IP {IP}.
                     Somewthing went wrong in [GroupManagment Module]-[GroupManagment_GroupInvitationsRepository class]-[AcceptInvitation Method]
                     User with id {invitedUserId} tried to accept the invitation with id {invitationId} made by 
                     the user with id {invitingUserId} for the group with id {groupId} but the invitation with that 
                     id doesn't exists. Dto {@DTO}
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                accepInvitation.InvitedUserId,
                accepInvitation.InvitationId,
                accepInvitation.InvitingUserId,
                accepInvitation.GroupId,
                accepInvitation
            );

            return Util_GenericResponse<bool>.Response
            (
                false,
                false,
                "Something went wrong!",
                null,
                System.Net.HttpStatusCode.NotFound
            );
        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<bool, GroupManagment_GroupInvitationsRepository>.ReturnInternalServerError
            (
               ex,
               _logger,
               $"""
                    Somewthing went wrong in [GroupManagment Module]-[GroupManagment_GroupInvitationsRepository class]-[AcceptInvitation Method],
                    user with [ID] {accepInvitation.InvitedUserId} tried to accept the invitation by user with id {accepInvitation.InvitingUserId} for the 
                    group with id {accepInvitation.GroupId} with the invitation id {accepInvitation.InvitationId}.
                """,
               false,
               _httpContextAccessor
            );
        }
    }
    /// <summary>
    /// Rejects a received group invitation.
    /// </summary>
    /// <param name="rejectInvitation">The details of the invitation being rejected.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a generic response indicating the success or failure of rejecting the invitation.</returns>
    public async Task<Util_GenericResponse<bool>>
    RejectInvitation
    (
        DTO_InvitationRequestActions rejectInvitation
    )
    {
        try
        {
            var checkPassed = await GenerealChecks("RejectInvitation", rejectInvitation.GroupId, rejectInvitation.InvitingUserId, rejectInvitation.InvitedUserId);

            if (!checkPassed)
            {
                return Util_GenericResponse<bool>.Response
                (
                   false,
                   false,
                   "Something went wrong!",
                   null,
                   System.Net.HttpStatusCode.BadRequest
                );
            }

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

                _logger.Log
                (
                    LogLevel.Information,
                    """
                         [GroupManagment Module]-[GroupManagment_GroupInvitationsRepository class]-[RejectInvitation Method].
                         Request with IP {IP}.
                         User with id {invitedUserId} rejected the invitation with id {invitationId} made by 
                         the user with id {invitingUserId} for the group with id {groupId}. Dto {@DTO}
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    rejectInvitation.InvitedUserId,
                    rejectInvitation.InvitationId,
                    rejectInvitation.InvitingUserId,
                    rejectInvitation.GroupId,
                    rejectInvitation
                );

                return Util_GenericResponse<bool>.Response
                (
                   true,
                   true,
                   "Invitation rejected succsessfully",
                   null,
                   System.Net.HttpStatusCode.OK
                );
            }

            _logger.Log
            (
                LogLevel.Error,
                """
                     Somewthing went wrong in [GroupManagment Module]-[GroupManagment_GroupInvitationsRepository class]-[RejectInvitation Method].
                     Request with IP {IP}.
                     User with id {invitedUserId} tried to reject the invitation with id {invitationId} made by 
                     the user with id {invitingUserId} for the group with id {groupId} but the invitation with that 
                     id doesn't exists. Dto {@DTO}
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                rejectInvitation.InvitedUserId,
                rejectInvitation.InvitationId,
                rejectInvitation.InvitingUserId,
                rejectInvitation.GroupId,
                rejectInvitation
            );

            return Util_GenericResponse<bool>.Response
            (
               false,
               false,
               "Something went wrong!",
               null,
               System.Net.HttpStatusCode.NotFound
            );

        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<bool, GroupManagment_GroupInvitationsRepository>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                    Somewthing went wrong in [GroupManagment Module]-[GroupManagment_GroupInvitationsRepository class]-[RejectInvitation Method],
                    user with [ID] {rejectInvitation.InvitedUserId} tried to reject the invitation by user with id {rejectInvitation.InvitingUserId} for the 
                    group with id {rejectInvitation.GroupId} with the invitation id {rejectInvitation.InvitationId}.
                 """,
                false,
                _httpContextAccessor
            );
        }
    }
    /// <summary>
    /// Deletes a previously sent group invitation.
    /// </summary>
    /// <param name="deleteInvitation">The details of the invitation being deleted.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a generic response indicating the success or failure of deleting the invitation.</returns>
    public async Task<Util_GenericResponse<bool>>
    DeleteSentInvitation
    (
        DTO_InvitationRequestActions deleteInvitation
    )
    {
        try
        {
            var checkPassed = await GenerealChecks("DeleteSentInvitation", deleteInvitation.GroupId, deleteInvitation.InvitingUserId, deleteInvitation.InvitedUserId);

            if (!checkPassed)
            {
                return Util_GenericResponse<bool>.Response
                (
                   false,
                   false,
                   "Something went wrong!",
                   null,
                   System.Net.HttpStatusCode.BadRequest
                );
            }

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

                _logger.Log
                (
                    LogLevel.Information,
                    """
                        [GroupManagment Module]-[GroupManagment_GroupInvitationsRepository class]-[DeleteSentInvitation Method]
                        Request with IP {IP}.
                        User with id {invitingUserId} deleted the invitation with id {invitationId} made to  
                        the user with id {invitedUserId} for the group with id {groupId}. Dto {@DTO}
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    deleteInvitation.InvitingUserId,
                    deleteInvitation.InvitationId,
                    deleteInvitation.InvitedUserId,
                    deleteInvitation.GroupId,
                    deleteInvitation
                );

                return Util_GenericResponse<bool>.Response
                (
                    true,
                    true,
                    "Invitation deleted succsessfully",
                    null,
                    System.Net.HttpStatusCode.OK
                );
            }

            _logger.Log
            (
                LogLevel.Error,
                """
                    Somewthing went wrong in [GroupManagment Module]-[GroupManagment_GroupInvitationsRepository class]-[DeleteSentInvitation Method]
                    Request with IP {IP}.
                    User with id {invitingUserId} tried to delete the invitation with id {invitationId} made to  
                    the user with id {invitedUserId} for the group with id {groupId} but the invitation with that 
                    id doesn't exists. Dto {@DTO}
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                deleteInvitation.InvitingUserId,
                deleteInvitation.InvitationId,
                deleteInvitation.InvitedUserId,
                deleteInvitation.GroupId,
                deleteInvitation
            );

            return Util_GenericResponse<bool>.Response
            (
                false,
                false,
                "Something went wrong!",
                null,
                System.Net.HttpStatusCode.NotFound
            );

        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<bool, GroupManagment_GroupInvitationsRepository>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                    Somewthing went wrong in [GroupManagment Module]-[GroupManagment_GroupInvitationsRepository class]-[DeleteSentInvitation Method],
                    user with [ID] {deleteInvitation.InvitingUserId} tried to delete the invitation to the user with id {deleteInvitation.InvitedUserId} for the 
                    group with id {deleteInvitation.GroupId} with the invitation id {deleteInvitation.InvitationId}.
                 """,
                false,
                _httpContextAccessor
            );
        }
    }
    /// <summary>
    /// Performs general checks for various invitation operations.
    /// </summary>
    /// <param name="methodName">The name of the method calling this function.</param>
    /// <param name="groupId">The unique identifier of the group involved in the operation.</param>
    /// <param name="invitingUserId">The unique identifier of the inviting user.</param>
    /// <param name="invitedUserId">The unique identifier of the invited user.</param>
    /// <returns>A task representing the asynchronous operation. The task result is a boolean indicating whether the checks passed or failed.</returns>
    private async Task<bool>
    GenerealChecks
    (
        string methodName,
        Guid groupId,
        Guid invitingUserId,
        Guid invitedUserId
    )
    {
        var group = await _db.Groups.FirstOrDefaultAsync(x => x.Id == groupId && !x.IsDeleted);

        if (group is null)
        {
            _logger.Log
            (
                LogLevel.Error,
                """
                    [GroupManagment Module]--[GroupManagment_GroupInvitationsRepository class]--[{methodName} Method] => 
                    [RESULT] : [IP] {IP} user with [ID] {ID} invited user with id {InvitedUserId} to a non existing or deleted group.
                 """,
                methodName,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                invitingUserId,
                invitedUserId
            );

            return false;
        }

        var invitingUser = await _db.Users.FirstOrDefaultAsync(x => x.Id == invitingUserId.ToString() && !x.IsDeleted);

        if (invitingUser is null)
        {
            _logger.Log
            (
                LogLevel.Error,
                """
                    [GroupManagment Module]--[GroupManagment_GroupInvitationsRepository class]--[{methodName} Method] => 
                    [RESULT] : [IP] {IP}. Inviting user with [ID] {ID} doesn't exists.
                 """,
                methodName,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                invitingUserId
            );

            return false;
        }

        var invitedUser = await _db.Users.FirstOrDefaultAsync(x => x.Id == invitedUserId.ToString() && !x.IsDeleted);

        if (invitedUser is null)
        {
            _logger.Log
            (
                LogLevel.Error,
                """
                    [GroupManagment Module]--[GroupManagment_GroupInvitationsRepository class]--[{methodName} Method] => 
                    [RESULT] : [IP] {IP} inviting user with [ID] {ID} send an invitation to user with id {invitedUser},
                    who doesn't exists or is deleted.
                 """,
                methodName,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                invitingUserId,
                invitedUserId
            );

            return false;
        }

        return true;
    }
}