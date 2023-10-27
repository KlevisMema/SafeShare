/* 
 * Defines a MediatR command for changing a user's password.
*/

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.UserManagment;

namespace SafeShare.MediatR.Actions.Commands.UserManagment;

/// <summary>
/// Represents a MediatR command to change a user's password based on their ID and provided password details.
/// </summary>
public class MediatR_ChangeUserPasswordCommand : IRequest<ObjectResult>
{
    /// <summary>
    /// Gets or sets the ID of the user whose password is to be changed.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Gets or sets the data transfer object containing the details required to update the user's password.
    /// </summary>
    public DTO_UserChangePassword UpdatePasswordDto { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_ChangeUserPasswordCommand"/> class with the specified user ID and password details.
    /// </summary>
    /// <param name="id">The ID of the user whose password is to be changed.</param>
    /// <param name="updatePasswordDto">The data transfer object containing the details required to update the password.</param>
    public MediatR_ChangeUserPasswordCommand
    (
        Guid id,
        DTO_UserChangePassword updatePasswordDto
    )
    {
        Id = id;
        UpdatePasswordDto = updatePasswordDto;
    }
}