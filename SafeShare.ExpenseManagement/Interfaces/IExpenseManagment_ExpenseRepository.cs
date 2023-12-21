/*
 * This interface declares the contract for the Expense Management Repository. It defines methods
 * for managing expenses within the application, such as retrieving all expenses for a group, 
 * getting a specific expense, creating, editing, and deleting expenses.
*/

using SafeShare.DataTransormObject.SafeShareApi.ExpenseManagment;
using SafeShare.Utilities.SafeShareApi.Responses;

namespace SafeShare.ExpenseManagement.Interfaces;

/// <summary>
/// Provides methods for managing expenses within the application's groups, 
/// including creating, retrieving, updating, and deleting expenses.
/// </summary>
public interface IExpenseManagment_ExpenseRepository
{
    /// <summary>
    /// Retrieves all expenses for a specified group accessible by the given user.
    /// </summary>
    /// <param name="groupId">The ID of the group.</param>
    /// <param name="userId">The ID of the user accessing the expenses.</param>
    /// <returns>A generic response containing the list of expense DTOs.</returns>
    Task<Util_GenericResponse<List<DTO_Expense>>>
    GetAllExpensesOfGroup
    (
        Guid groupId,
        string userId
    );
    /// <summary>
    /// Retrieves a single expense by its ID that is accessible by the given user.
    /// </summary>
    /// <param name="expenseId">The ID of the expense to retrieve.</param>
    /// <param name="userId">The ID of the user accessing the expense.</param>
    /// <returns>A generic response containing the expense DTO.</returns>
    Task<Util_GenericResponse<DTO_Expense>>
    GetExpense
    (
        Guid expenseId,
        string userId
    );
    /// <summary>
    /// Creates a new expense within a group and updates the balances of all group members.
    /// </summary>
    /// <param name="expenseDto">The DTO containing the expense creation data.</param>
    /// <param name="userId">The ID of the user creating the expense.</param>
    /// <returns>A generic response containing the created expense DTO.</returns>
    Task<Util_GenericResponse<DTO_Expense>>
    CreateExpense
    (
        DTO_ExpenseCreate expenseDto,
        string userId
    );
    /// <summary>
    /// Edits an existing expense and updates the group members' balances as needed.
    /// </summary>
    /// <param name="expenseId">The ID of the expense to be edited.</param>
    /// <param name="expenseCreateDto">The DTO containing the updated expense data.</param>
    /// <param name="userId">The ID of the user performing the edit operation.</param>
    /// <returns>A generic response containing the edited expense DTO.</returns>
    Task<Util_GenericResponse<DTO_Expense>>
    EditExpense
    (
        Guid expenseId,
        DTO_ExpenseCreate expenseCreateDto,
        string userId
    );
    /// <summary>
    /// Deletes an expense from the group and updates the balances of all affected members.
    /// </summary>
    /// <param name="expenseDelete">The DTO containing the data for the expense to be deleted.</param>
    /// <returns>A generic response indicating the success or failure of the delete operation.</returns>
    Task<Util_GenericResponse<bool>>
    DeleteExpense
    (
        DTO_ExpenseDelete expenseDelete
    );
}