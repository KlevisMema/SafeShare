/*
 * Defines a command for creating a new expense within the system.
 * This command includes the user ID of the member creating the expense and the data transfer object
 * containing the details of the expense. It is intended to be processed by a MediatR request handler
 * that will handle the logic for adding the new expense to the database and updating any associated
 * data, such as group member balances.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.SafeShareApi.ExpenseManagment;

namespace SafeShare.MediatR.Actions.Commands.ExpenseManagment;

/// <summary>
/// Represents a command for MediatR to create a new expense entry.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_CreateExpenseCommand"/> class.
/// </remarks>
/// <param name="userId">The unique identifier for the user creating the expense.</param>
/// <param name="expenseDto">The DTO containing the expense creation data.</param>
public class MediatR_CreateExpenseCommand
(
    Guid userId,
    DTO_ExpenseCreate expenseDto
) : IRequest<ObjectResult>
{
    /// <summary>
    /// Gets or sets the unique identifier for the user who is creating the expense.
    /// </summary>
    public Guid UserId { get; set; } = userId;
    /// <summary>
    /// Gets or sets the Data Transfer Object (DTO) containing the information for the expense to be created.
    /// </summary>
    public DTO_ExpenseCreate ExpenseDto { get; set; } = expenseDto;
}