using SafeShare.Utilities.Responses;
using SafeShare.DataTransormObject.UserManagment;

namespace SafeShare.UserManagment.Interfaces;

public interface IAccountManagment
{
    Task<Util_GenericResponse<DTO_UserUpdatedInfo>>
    GetUser
    (
        Guid id
    );

    Task<Util_GenericResponse<DTO_UserUpdatedInfo>>
    UpdateUser
    (
        Guid id,
        DTO_UserInfo dtoUser
    );

    Task<Util_GenericResponse<bool>>
    DeleteUser
    (
        Guid id
    );

    Task<Util_GenericResponse<bool>>
    ChangePassword
    (
        Guid id,
        DTO_UserChangePassword updatePassword
    );
}