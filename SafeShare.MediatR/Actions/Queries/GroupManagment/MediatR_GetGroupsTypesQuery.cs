/* 
 * Defines a MediatR query for retrieving the types of groups associated with a specific user.
 * This query is intended for use within MediatR handlers to fetch information about various groups that the user is a part of, categorized by their types.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SafeShare.MediatR.Actions.Queries.GroupManagment;

/// <summary>
/// Represents a MediatR query for retrieving the types of groups associated with a specific user.
/// This query includes the user's identifier to fetch group types the user is involved with.
/// </summary>
public class MediatR_GetGroupsTypesQuery : IRequest<ObjectResult>
{
    /// <summary>
    /// The unique identifier of the user for whom group types are being queried.
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_GetGroupsTypesQuery"/> class.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to fetch group types for.</param>
    public MediatR_GetGroupsTypesQuery
    (
        Guid userId
    )
    {
        UserId = userId;
    }
}