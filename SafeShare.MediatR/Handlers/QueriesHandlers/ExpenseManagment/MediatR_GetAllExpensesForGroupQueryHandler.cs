/*
 * Handles the retrieval of all expenses for a specified group using MediatR as part of the CQRS pattern.
 * This handler processes the MediatR_GetAllExpensesForGroupQuery, invoking the expense management
 * service to fetch and return all expense records for a group, ensuring that only authorized users can
 * access the group's expense information.
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
/// MediatR query handler responsible for retrieving all expenses associated with a specific group.
/// </summary>
public class MediatR_GetAllExpensesForGroupQueryHandler :
    MediatR_GenericHandler<IExpenseManagment_ExpenseRepository>,
    IRequestHandler<MediatR_GetAllExpensesForGroupQuery, ObjectResult>
{
    /// <summary>
    /// Constructs a new instance of the <see cref="MediatR_GetAllExpensesForGroupQueryHandler"/> with the necessary expense management service.
    /// </summary>
    /// <param name="service">The expense management service to be used within this handler.</param>
    public MediatR_GetAllExpensesForGroupQueryHandler
    (
        IExpenseManagment_ExpenseRepository service
    )
    : base
    (
        service
    )
    { }
    /// <summary>
    /// Asynchronously handles the retrieval of all expenses for a group upon receiving the query.
    /// It delegates to the expense management service to perform the actual data fetching.
    /// </summary>
    /// <param name="request">The query that includes the group ID and the user ID for which expenses are to be retrieved.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to abort the handling of the query.</param>
    /// <returns>An ObjectResult that encapsulates the list of expenses retrieved for the group.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_GetAllExpensesForGroupQuery request,
        CancellationToken cancellationToken
    )
    {
        var getAllExpensesForGroupResult = await _service.GetAllExpensesOfGroup(request.GroupId, request.UserId.ToString());

        return Util_GenericControllerResponse<List<DTO_Expense>>.ControllerResponse(getAllExpensesForGroupResult);
    }
}