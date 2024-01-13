/*
 * Encapsulates a command request for editing an existing expense within the system using MediatR.
 * The command carries the necessary data to identify the user and the specific expense to be edited,
 * along with the new expense data. This command is designed to work with the CQRS pattern, separating
 * the command (write) concerns from the query (read) concerns.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.SafeShareApi.ExpenseManagment;

namespace SafeShare.MediatR.Actions.Commands.ExpenseManagment;

/// <summary>
/// Represents a command for updating an existing expense with new details.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_EditExpenseCommand"/> class with specified user ID, expense ID, and updated expense details.
/// </remarks>
/// <param name="userId">The ID of the user performing the edit operation.</param>
/// <param name="expenseId">The ID of the expense to be edited.</param>
/// <param name="expenseCreateDto">The DTO containing the updated expense information.</param>
public class MediatR_EditExpenseCommand
(
    Guid userId,
    Guid expenseId,
    DTO_ExpenseCreate expenseCreateDto
) : IRequest<ObjectResult>
{
    /// <summary>
    /// Gets or sets the ID of the user attempting to edit the expense.
    /// </summary>
    public Guid UserId { get; set; } = userId;
    /// <summary>
    /// Gets or sets the ID of the expense that is to be edited.
    /// </summary>
    public Guid ExpenseId { get; set; } = expenseId;
    /// <summary>
    /// Gets or sets the data transfer object containing the updated expense details.
    /// </summary>
    public DTO_ExpenseCreate ExpenseCreateDto { get; set; } = expenseCreateDto;
}