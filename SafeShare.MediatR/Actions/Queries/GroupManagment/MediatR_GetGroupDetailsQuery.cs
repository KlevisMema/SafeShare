/* 
 * Defines a MediatR query for retrieving detailed information about a specific group.
 * This query is designed to be processed by MediatR handlers to fetch comprehensive details of a group, including its members and activities, based on the group's ID and the querying user's ID.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SafeShare.MediatR.Actions.Queries.GroupManagment;

/// <summary>
/// Represents a MediatR query for retrieving detailed information about a specific group.
/// This query includes the identifiers for both the group and the user making the query to fetch comprehensive group details.
/// </summary>
public class MediatR_GetGroupDetailsQuery : IRequest<ObjectResult>
{
    /// <summary>
    /// The unique identifier of the group for which details are being queried.
    /// </summary>
    public Guid GroupId { get; set; }
    /// <summary>
    /// The unique identifier of the user making the query.
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_GetGroupDetailsQuery"/> class.
    /// </summary>
    /// <param name="userId">The unique identifier of the user making the query.</param>
    /// <param name="groupId">The unique identifier of the group for which details are being queried.</param>
    public MediatR_GetGroupDetailsQuery
    (
        Guid userId,
        Guid groupId
    )
    {
        UserId = userId;
        GroupId = groupId;
    }
}