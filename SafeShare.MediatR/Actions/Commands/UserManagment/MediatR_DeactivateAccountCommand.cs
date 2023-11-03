/* 
 * Defines a MediatR command for deactivating a user.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.UserManagment;

namespace SafeShare.MediatR.Actions.Commands.UserManagment;

/// <summary>
/// Represents a MediatR command to delete a user based on their ID.
/// </summary>
public class MediatR_DeactivateAccountCommand : IRequest<ObjectResult>
{
    /// <summary>
    /// Gets or sets the ID of the user to be deactivated.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Gets or sets the <see cref="DTO_DeactivateAccount"/> object of the user to be deactivated.
    /// </summary>
    public DTO_DeactivateAccount DeactivateAccount { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_DeactivateAccountCommand"/> class with the specified user ID.
    /// </summary>
    /// <param name="id">The ID of the user to be deactivated.</param>
    public MediatR_DeactivateAccountCommand
    (
        Guid id,
        DTO_DeactivateAccount deactivateAccount
    )
    {
        Id = id;
        DeactivateAccount = deactivateAccount;
    }
}