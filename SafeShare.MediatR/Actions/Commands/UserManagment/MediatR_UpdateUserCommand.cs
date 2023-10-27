/* 
 * Defines a MediatR command for updating a user.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.UserManagment;

namespace SafeShare.MediatR.Actions.Commands.UserManagment;

/// <summary>
/// Represents a MediatR command to update user information.
/// </summary>
public class MediatR_UpdateUserCommand : IRequest<ObjectResult>
{
    /// <summary>
    /// Gets or sets the ID of the user to be updated.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Gets or sets the user information to be updated.
    /// </summary>
    public DTO_UserInfo DTO_UserInfo { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_UpdateUserCommand"/> class with the specified user ID and information.
    /// </summary>
    /// <param name="id">The ID of the user to be updated.</param>
    /// <param name="dTO_UserInfo">The user information to be updated.</param>
    public MediatR_UpdateUserCommand
    (
        Guid id,
        DTO_UserInfo dTO_UserInfo
    )
    {
        Id = id;
        DTO_UserInfo = dTO_UserInfo;
    }
}