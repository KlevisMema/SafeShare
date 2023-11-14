/*
 * Represents a query to retrieve a specific expense from the system using MediatR.
 * The query includes the user ID of the requester and the expense ID of the target expense.
 * This query is designed to be processed by a MediatR query handler which will fetch the 
 * requested expense data, ensuring the requester has the necessary permissions to view it.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SafeShare.MediatR.Actions.Queries.ExpenseManagment;

/// <summary>
/// MediatR query object that requests retrieval of a single expense based on its ID.
/// </summary>
public class MediatR_GetExpenseQuery : IRequest<ObjectResult>
{
    /// <summary>
    /// Gets or sets the ID of the expense to be retrieved.
    /// </summary>
    public Guid ExpenseId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user making the query.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_GetExpenseQuery"/> class.
    /// </summary>
    /// <param name="userId">The ID of the user requesting the expense information.</param>
    /// <param name="expenseId">The ID of the expense to be retrieved.</param>
    public MediatR_GetExpenseQuery
    (
        Guid userId,
        Guid expenseId
    )
    {
        UserId = userId;
        ExpenseId = expenseId;
    }
}