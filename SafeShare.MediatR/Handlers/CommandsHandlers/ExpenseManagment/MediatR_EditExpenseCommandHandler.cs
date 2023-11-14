/*
 * Handles the editing of an existing expense using MediatR as part of the CQRS pattern. This handler takes
 * a MediatR_EditExpenseCommand, using the expense management service to update the details of an expense
 * within the system and ensure the corresponding data such as group member balances are adjusted accordingly.
 * This approach keeps command handling separate from core business logic.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.ExpenseManagement.Interfaces;
using SafeShare.MediatR.Actions.Commands.ExpenseManagment;
using SafeShare.DataTransormObject.Expenses;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.ExpenseManagment;

/// <summary>
/// MediatR command handler responsible for processing expense edit requests.
/// </summary>
public class MediatR_EditExpenseCommandHandler :
    MediatR_GenericHandler<IExpenseManagment_ExpenseRepository>,
    IRequestHandler<MediatR_EditExpenseCommand, ObjectResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_EditExpenseCommandHandler"/> with a service capable of handling expense management operations.
    /// </summary>
    /// <param name="service">The expense management service to be used within this handler.</param>
    public MediatR_EditExpenseCommandHandler
    (
        IExpenseManagment_ExpenseRepository service
    )
    : base
    (
        service
    )
    { }

    /// <summary>
    /// Asynchronously handles the editing of an expense upon receiving the edit command.
    /// It delegates to the expense management service to perform the actual update.
    /// </summary>
    /// <param name="request">The command that includes details about the expense to be edited and the user performing the edit.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to abort the handling of the command.</param>
    /// <returns>An ObjectResult that encapsulates the result of the edit operation.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_EditExpenseCommand request,
        CancellationToken cancellationToken
    )
    {
        var editExpenseResult = await _service.EditExpense(request.ExpenseId, request.ExpenseCreateDto, request.UserId.ToString());

        return Util_GenericControllerResponse<DTO_Expense>.ControllerResponse(editExpenseResult);
    }
}