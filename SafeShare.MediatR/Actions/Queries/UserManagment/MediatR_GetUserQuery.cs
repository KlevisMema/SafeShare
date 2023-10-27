/* 
 * Defines a MediatR query for retrieving a user based on their ID.
*/

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SafeShare.MediatR.Actions.Queries.UserManagment;

/// <summary>
/// Represents a MediatR query to fetch a user by their ID.
/// </summary>
public class MediatR_GetUserQuery : IRequest<ObjectResult>
{
    /// <summary>
    /// Gets or sets the ID of the user to be retrieved.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_GetUserQuery"/> class with the specified user ID.
    /// </summary>
    /// <param name="id">The ID of the user to be retrieved.</param>
    public MediatR_GetUserQuery
    (
        Guid id
    )
    {
        Id = id;
    }
}