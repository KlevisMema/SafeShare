/* 
 * Defines a MediatR query for retrieving group invitation information for a specific user.
 * This query is designed to be handled by MediatR to fetch all group invitations sent to a particular user, identified by their user ID.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SafeShare.MediatR.Actions.Queries.GroupManagment;

/// <summary>
/// Represents a MediatR query for retrieving group invitations sent to a specific user.
/// This query includes the user's identifier to fetch the relevant group invitations.
/// </summary>
public class MediatR_GetGetGroupsInvitationsQuery : IRequest<ObjectResult>
{
    /// <summary>
    /// The unique identifier of the user for whom group invitations are being queried.
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_GetGetGroupsInvitationsQuery"/> class.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to fetch group invitations for.</param>
    public MediatR_GetGetGroupsInvitationsQuery
    (
        Guid userId
    )
    {
        UserId = userId;
    }
}