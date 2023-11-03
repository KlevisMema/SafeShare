using SafeShare.Utilities.Responses;
using SafeShare.DataTransormObject.GroupManagment;

namespace SafeShare.GroupManagment.Interfaces;

public interface IGroupManagment_GroupRepository
{
    Task<Util_GenericResponse<DTO_GroupType>>
    CreateGroup
    (
        Guid ownerId,
        DTO_CreateGroup createGroup
    );

    Task<Util_GenericResponse<DTO_GroupsTypes>>
    GetGroupsTypes
    (
        Guid userId
    );

    Task<Util_GenericResponse<DTO_GroupDetails>>
    GetGroupDetails
    (
        Guid userId,
        Guid groupId
    );

    Task<Util_GenericResponse<DTO_GroupType>>
    EditGroup
    (
        Guid userId,
        Guid groupId,
        DTO_EditGroup editGroup
    );

    Task<Util_GenericResponse<bool>>
    DeleteGroup
    (
        Guid ownerId,
        Guid groupId
    );
}