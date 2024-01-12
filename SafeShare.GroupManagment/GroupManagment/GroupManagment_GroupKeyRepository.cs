using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SafeShare.Security.GroupSecurity;
using SafeShare.DataAccessLayer.Context;
using SafeShare.Utilities.SafeShareApi.IP;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.Utilities.SafeShareApi.Dependencies;
using SafeShare.DataAccessLayer.Models.SafeShareApi.CryptoModels;

namespace SafeShare.GroupManagment.GroupManagment;

public class GroupManagment_GroupKeyRepository
(
    CryptoKeysDb db,
    IGroupKeySecurity groupKeySecurity,
    ILogger<GroupManagment_GroupKeyRepository> logger

) : Util_BaseContextDependencies<CryptoKeysDb, GroupManagment_GroupKeyRepository>(
    db,
    null!,
    logger,
    null!
), IGroupManagment_GroupKeyRepository
{
    public async Task<bool>
    CreateKeyForGroup
    (
        Guid groupId
    )
    {
        try
        {
            var groupKey = new GroupKey
            {
                GroupId = groupId,
                KeyCreatedTime = DateTime.UtcNow,
                KeyUpdatedTime = null,
                CryptoKey = groupKeySecurity.ProtectCryptoKey(groupId),
            };

            await db.GroupKeys.AddAsync(groupKey);
            await db.SaveChangesAsync();

            _logger.Log
            (
                LogLevel.Information,
                """
                    [GroupManagment Module]--[GroupManagment_GroupKeyRepository class]--[CreateKeyForGroup Method] => 
                    [RESULT] : A key was successfully created for group with id {groupId}.
                 """,
                groupId
            );

            return true;

        }
        catch (Exception ex)
        {
            _logger.Log
            (
                LogLevel.Critical,
                """
                    [GroupManagment Module]--[GroupManagment_GroupKeyRepository class]--[CreateKeyForGroup Method] => 
                    [RESULT] : A key could not be created for the group with id {groupId}. An excpetion was thrown=>
                    {ex}
                 """,
                groupId,
                ex
            );

            return false;
        }
    }

    public async Task<bool>
    DeleteGroupKey
    (
        Guid groupId
    )
    {
        try
        {
            var groupKey = await db.GroupKeys.FirstOrDefaultAsync(x => x.GroupId == groupId);

            if (groupKey is null)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [GroupManagment Module]--[GroupManagment_GroupKeyRepository class]--[DeleteGroupKey Method] => 
                        [RESULT] : A key was not found for group with id {groupId}.
                    """,
                    groupId
                );

                return false;
            }

            db.GroupKeys.Remove(groupKey);
            await db.SaveChangesAsync();

            _logger.Log
            (
                LogLevel.Information,
                """
                    [GroupManagment Module]--[GroupManagment_GroupKeyRepository class]--[DeleteGroupKey Method] => 
                    [RESULT] : A key was successfully deleted for group with id {groupId}.
                """,
                groupId
            );

            return true;

        }
        catch (Exception ex)
        {
            _logger.Log
            (
                LogLevel.Critical,
                """
                    [GroupManagment Module]--[GroupManagment_GroupKeyRepository class]--[DeleteGroupKey Method] => 
                    [RESULT] : A key could not be deleted for the group with id {groupId}. An excpetion was thrown=>
                    {ex}
                """,
                groupId,
                ex
            );

            return false;
        }
    }
}