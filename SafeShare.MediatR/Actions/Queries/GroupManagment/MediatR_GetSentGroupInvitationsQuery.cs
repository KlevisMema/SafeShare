/* 
 * Defines a MediatR query for retrieving information about group invitations sent by a specific user.
 * This query is designed to be handled by MediatR to fetch details of all the invitations sent out by the user, identified by their user ID.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SafeShare.MediatR.Actions.Queries.GroupManagment;

/// <summary>
/// Represents a MediatR query for retrieving information about group invitations sent by a specific user.
/// This query includes the user's identifier to fetch the details of sent group invitations.
/// </summary>
public class MediatR_GetSentGroupInvitationsQuery : IRequest<ObjectResult>
{
    /// <summary>
    /// The unique identifier of the user who has sent group invitations.
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_GetSentGroupInvitationsQuery"/> class.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose sent group invitations are to be fetched.</param>
    public MediatR_GetSentGroupInvitationsQuery
    (
        Guid userId
    )
    {
        UserId = userId;
    }
}