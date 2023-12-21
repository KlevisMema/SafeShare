/*
 * API Controller for managing expenses within the application. It provides endpoints for retrieving all expenses for a group,
 * getting a specific expense, creating, editing, and deleting expenses. The controller uses MediatR to send commands and queries,
 * which are handled by corresponding handlers. This separates the HTTP request handling from the business logic.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SafeShare.DataAccessLayer.Models;
using SafeShare.ClientServerShared.Routes;
using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.MediatR.Actions.Queries.ExpenseManagment;
using SafeShare.MediatR.Actions.Commands.ExpenseManagment;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment;
using SafeShare.DataTransormObject.SafeShareApi.ExpenseManagment;

namespace SafeShare.API.Controllers;

/// <summary>
/// Controller responsible for providing endpoints related to expense management.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ExpenseManagmentController"/> with the MediatR mediator.
/// </remarks>
/// <param name="mediator">The MediatR mediator for sending commands and queries.</param>
public class ExpenseManagmentController(IMediator mediator) : BaseController(mediator)
{
    /// <summary>
    /// Endpoint for retrieving all expenses of a specific group.
    /// </summary>
    /// <param name="groupId">The ID of the group whose expenses to retrieve.</param>
    /// <param name="userId">The ID of the user making the request.</param>
    /// <returns>A response with the list of expenses or an error message.</returns>
    [HttpGet(Route_ExpenseManagment.GetAllExpensesOfGroup)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<List<DTO_Expense>>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Util_GenericResponse<List<DTO_Expense>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<List<DTO_Expense>>))]
    public async Task<ActionResult<Util_GenericResponse<List<DTO_Expense>>>>
    GetAllExpensesOfGroup
    (
        Guid userId,
        Guid groupId
    )
    {
        return await _mediator.Send(new MediatR_GetAllExpensesForGroupQuery(groupId, userId));
    }
    /// <summary>
    /// Endpoint for retrieving a specific expense by its ID.
    /// </summary>
    /// <param name="expenseId">The ID of the expense to retrieve.</param>
    /// <param name="userId">The ID of the user making the request.</param>
    /// <returns>A response with the expense data or an error message.</returns>
    [HttpGet(Route_ExpenseManagment.GetExpense)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<DTO_Expense>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<DTO_Expense>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Util_GenericResponse<DTO_Expense>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<DTO_Expense>))]
    public async Task<ActionResult<Util_GenericResponse<DTO_Expense>>>
    GetExpense
    (
        Guid expenseId,
        Guid userId
    )
    {
        return await _mediator.Send(new MediatR_GetExpenseQuery(userId, expenseId));
    }
    /// <summary>
    /// Endpoint for creating a new expense.
    /// </summary>
    /// <param name="expenseDto">The DTO containing the new expense data.</param>
    /// <param name="userId">The ID of the user creating the expense.</param>
    /// <returns>A response indicating the result of the creation operation.</returns>
    [HttpPost(Route_ExpenseManagment.CreateExpense)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<DTO_Expense>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Util_GenericResponse<DTO_Expense>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<DTO_Expense>))]
    public async Task<ActionResult<Util_GenericResponse<DTO_Expense>>>
    CreateExpense
    (
        [FromForm] DTO_ExpenseCreate expenseDto,
        Guid userId
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_CreateExpenseCommand(userId, expenseDto));
    }
    /// <summary>
    /// Endpoint for editing an existing expense.
    /// </summary>
    /// <param name="userId">The ID of the user editing the expense.</param>
    /// <param name="expenseId">The ID of the expense to be edited.</param>
    /// <param name="expenseCreateDto">The DTO containing the updated expense data.</param>
    /// <returns>A response indicating the result of the edit operation.</returns>
    [HttpPut(Route_ExpenseManagment.EditExpense)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<DTO_Expense>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<DTO_Expense>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Util_GenericResponse<DTO_Expense>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<DTO_Expense>))]
    public async Task<ActionResult<Util_GenericResponse<DTO_Expense>>>
    EditExpense
    (
        Guid userId,
        Guid expenseId,
        DTO_ExpenseCreate expenseCreateDto
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_EditExpenseCommand(userId, expenseId, expenseCreateDto));
    }
    /// <summary>
    /// Endpoint for deleting an expense.
    /// </summary>
    /// <param name="expenseDelete">The DTO containing the ID of the expense to be deleted and the user ID of the requester.</param>
    /// <param name="userId">The id of the user making the request</param>
    /// <returns>A response indicating the result of the delete operation.</returns>
    [HttpDelete(Route_ExpenseManagment.DeleteExpense)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<bool>))]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    DeleteExpense
    (
        Guid userId,
        DTO_ExpenseDelete expenseDelete
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_DeleteExpenseCommand(expenseDelete));
    }
}