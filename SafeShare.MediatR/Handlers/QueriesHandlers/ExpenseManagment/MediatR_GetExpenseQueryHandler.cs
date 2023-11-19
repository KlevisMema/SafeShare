/*
 * Handles the retrieval of a single expense by its ID using MediatR within the CQRS pattern.
 * This query handler processes the MediatR_GetExpenseQuery and uses the expense management
 * service to obtain specific expense data. The handler ensures that the expense data is returned
 * only if the requesting user has the appropriate permissions to view it.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.DataTransormObject.Expenses;
using SafeShare.ExpenseManagement.Interfaces;
using SafeShare.MediatR.Actions.Queries.ExpenseManagment;

namespace SafeShare.MediatR.Handlers.QueriesHandlers.ExpenseManagment;

/// <summary>
/// MediatR query handler responsible for processing requests to retrieve a single expense.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_GetExpenseQueryHandler"/> class with the provided expense management service.
/// </remarks>
/// <param name="service">The expense management service used to fetch expense data.</param>
public class MediatR_GetExpenseQueryHandler
(
    IExpenseManagment_ExpenseRepository service
) : MediatR_GenericHandler<IExpenseManagment_ExpenseRepository>(service),
    IRequestHandler<MediatR_GetExpenseQuery, ObjectResult>
{
    /// <summary>
    /// Handles the operation of retrieving a specific expense based on the provided query.
    /// </summary>
    /// <param name="request">The query containing the ID of the expense to retrieve and the user ID of the requester.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation if needed.</param>
    /// <returns>An ObjectResult containing the requested expense or an error message.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_GetExpenseQuery request,
        CancellationToken cancellationToken
    )
    {
        var getExpenseResult = await _service.GetExpense(request.ExpenseId, request.UserId.ToString());

        return Util_GenericControllerResponse<DTO_Expense>.ControllerResponse(getExpenseResult);
    }
}