/* 
 * Defines a MediatR command for deleting a user.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.UserManagment;

namespace SafeShare.MediatR.Actions.Commands.UserManagment;

/// <summary>
/// Represents a MediatR command to delete a user based on their ID.
/// </summary>
public class MediatR_DeleteUserCommand : IRequest<ObjectResult>
{
    /// <summary>
    /// Gets or sets the ID of the user to be deleted.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_DeleteUserCommand"/> class with the specified user ID.
    /// </summary>
    /// <param name="id">The ID of the user to be deleted.</param>
    public MediatR_DeleteUserCommand
    (
        Guid id
    )
    {
        Id = id;
    }
}