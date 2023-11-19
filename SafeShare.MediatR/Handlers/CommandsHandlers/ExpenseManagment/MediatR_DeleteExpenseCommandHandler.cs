/*
 * Handles the deletion of an expense using MediatR as part of the CQRS pattern. This handler takes
 * a MediatR_DeleteExpenseCommand, using the expense management service to remove an expense from the system
 * and update related data such as group member balances. This promotes a clean separation of command handling
 * from the business logic.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.ExpenseManagement.Interfaces;
using SafeShare.MediatR.Actions.Commands.ExpenseManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.ExpenseManagment;

/// <summary>
/// MediatR command handler for deleting an expense.
/// </summary>
/// <remarks>
/// Constructs a new instance of the <see cref="MediatR_DeleteExpenseCommandHandler"/> with the necessary expense management service.
/// </remarks>
/// <param name="service">The expense management service to be used within this handler.</param>
public class MediatR_DeleteExpenseCommandHandler
(
    IExpenseManagment_ExpenseRepository service
) : MediatR_GenericHandler<IExpenseManagment_ExpenseRepository>(service),
    IRequestHandler<MediatR_DeleteExpenseCommand, ObjectResult>
{
    /// <summary>
    /// Processes the incoming delete expense command, utilizing the expense management service to perform the deletion.
    /// </summary>
    /// <param name="request">The command containing the details of the expense to be deleted.</param>
    /// <param name="cancellationToken">A token that can be used to signal cancellation of the delete operation.</param>
    /// <returns>An ObjectResult encapsulating the outcome of the delete operation.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_DeleteExpenseCommand request,
        CancellationToken cancellationToken
    )
    {
        var deleteExpenseResult = await _service.DeleteExpense(request.ExpenseDelete);

        return Util_GenericControllerResponse<bool>.ControllerResponse(deleteExpenseResult);
    }
}