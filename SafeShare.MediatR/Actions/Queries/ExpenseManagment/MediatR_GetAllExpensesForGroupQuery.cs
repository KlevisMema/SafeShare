/*
 * Represents a query to retrieve all expenses for a specific group within the system using MediatR.
 * This query includes identifiers for the user requesting the information and the target group.
 * It is intended to be processed by a MediatR query handler that will fetch the appropriate expense
 * data from the system and return it for display or further processing.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SafeShare.MediatR.Actions.Queries.ExpenseManagment;

/// <summary>
/// MediatR query object that requests retrieval of all expenses associated with a specific group.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_GetAllExpensesForGroupQuery"/> class.
/// </remarks>
/// <param name="userId">The ID of the user performing the query.</param>
/// <param name="groupId">The ID of the group whose expenses are to be retrieved.</param>
public class MediatR_GetAllExpensesForGroupQuery
(
    Guid userId,
    Guid groupId
) : IRequest<ObjectResult>
{
    /// <summary>
    /// Gets or sets the ID of the group for which expenses are being queried.
    /// </summary>
    public Guid GroupId { get; set; } = groupId;
    /// <summary>
    /// Gets or sets the ID of the user making the query.
    /// </summary>
    public Guid UserId { get; set; } = userId;
}