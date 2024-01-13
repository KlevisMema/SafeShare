namespace SafeShare.GroupManagment.Interfaces;

public interface IGroupManagment_GroupKeyRepository
{
    Task<bool>
    CreateKeyForGroup
    (
        Guid tag,
        Guid groupId
    );

    Task<bool>
    UpdateKeyForGroup
    (
        Guid tag,
        Guid groupId
    );

    Task<bool>
    DeleteGroupKey
    (
        Guid groupId
    );
}