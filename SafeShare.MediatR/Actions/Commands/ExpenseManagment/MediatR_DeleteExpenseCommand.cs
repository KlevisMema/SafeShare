/*
 * Defines a command for deleting an existing expense within the system.
 * This command includes the data transfer object containing the details of the expense to be deleted.
 * It is intended to be processed by a MediatR request handler that will handle the logic for removing
 * the expense from the database and updating any associated data, such as group member balances.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.ExpenseManagment;

namespace SafeShare.MediatR.Actions.Commands.ExpenseManagment;

/// <summary>
/// Represents a command for MediatR to delete an existing expense entry.
/// </summary>
public class MediatR_DeleteExpenseCommand : IRequest<ObjectResult>
{
    /// <summary>
    /// Gets or sets the details of the expense to be deleted, encapsulated in a DTO.
    /// </summary>
    public DTO_ExpenseDelete ExpenseDelete { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_DeleteExpenseCommand"/> class with specified expense deletion details.
    /// </summary>
    /// <param name="expenseDelete">The data transfer object containing the expense deletion information.</param>
    public MediatR_DeleteExpenseCommand(DTO_ExpenseDelete expenseDelete)
    {
        ExpenseDelete = expenseDelete;
    }
}