/***
 * Handles the creation of an expense using MediatR as part of the CQRS pattern. This handler processes
 * the MediatR_CreateExpenseCommand, invoking the expense management service to create a new expense and
 * manage associated state changes such as group member balances. It ensures a separation of concerns where
 * the command handling logic is decoupled from the core business logic.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.MediatR.Dependencies;
using SafeShare.ExpenseManagement.Interfaces;
using SafeShare.MediatR.Actions.Commands.ExpenseManagment;
using SafeShare.DataTransormObject.SafeShareApi.ExpenseManagment;
using SafeShare.Utilities.SafeShareApi.Responses;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.ExpenseManagment;

/// <summary>
/// MediatR command handler for creating a new expense.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_CreateExpenseCommandHandler"/> class.
/// </remarks>
/// <param name="service">The expense management service to be used by the handler.</param>
public class MediatR_CreateExpenseCommandHandler
(
    IExpenseManagment_ExpenseRepository service
) : MediatR_GenericHandler<IExpenseManagment_ExpenseRepository>(service),
    IRequestHandler<MediatR_CreateExpenseCommand, ObjectResult>
{
    /// <summary>
    /// Handles the processing of a create expense command.
    /// </summary>
    /// <param name="request">The command containing the expense creation details.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the handler operation.</param>
    /// <returns>An ObjectResult that encapsulates the result of the create expense operation.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_CreateExpenseCommand request,
        CancellationToken cancellationToken
    )
    {
        var createExpenseResult = await _service.CreateExpense(request.ExpenseDto, request.UserId.ToString());

        return Util_GenericControllerResponse<DTO_Expense>.ControllerResponse(createExpenseResult);
    }
}