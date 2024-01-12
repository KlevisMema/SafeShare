namespace SafeShare.GroupManagment.Interfaces;

public interface IGroupManagment_GroupKeyRepository
{
    Task<bool>
    CreateKeyForGroup
    (
        Guid groupId
    );

    Task<bool>
    DeleteGroupKey
    (
        Guid groupId
    );
}