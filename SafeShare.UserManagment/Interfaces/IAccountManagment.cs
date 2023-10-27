/* 
 * This interface defines the contract for account management operations,
 * including retrieving user information, updating user details, 
 * deleting users, and changing user passwords.
*/

using SafeShare.Utilities.Responses;
using SafeShare.DataTransormObject.UserManagment;

namespace SafeShare.UserManagment.Interfaces;

/// <summary>
/// Provides an interface for account management operations.
/// </summary>
public interface IAccountManagment
{
    /// <summary>
    /// Fetches user information based on the provided user ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>A generic response containing the user's updated information.</returns>
    Task<Util_GenericResponse<DTO_UserUpdatedInfo>>
    GetUser
    (
        Guid id
    );
    /// <summary>
    /// Updates the user's information based on the provided user ID and user data.
    /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="dtoUser">The user's new information.</param>
    /// <returns>A generic response containing the user's updated information after the changes.</returns>
    Task<Util_GenericResponse<DTO_UserUpdatedInfo>>
    UpdateUser
    (
        Guid id,
        DTO_UserInfo dtoUser
    );
    /// <summary>
    /// Deletes the user based on the provided user ID.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns>A generic response indicating whether the user was successfully deleted or not.</returns>
    Task<Util_GenericResponse<bool>>
    DeleteUser
    (
        Guid id
    );
    /// <summary>
    /// Changes the password of a user based on the provided user ID and password details.
    /// </summary>
    /// <param name="id">The ID of the user whose password needs to be changed.</param>
    /// <param name="updatePassword">The details containing the old and new password information.</param>
    /// <returns>A generic response indicating whether the password was successfully changed or not.</returns>
    Task<Util_GenericResponse<bool>>
    ChangePassword
    (
        Guid id,
        DTO_UserChangePassword updatePassword
    );
}