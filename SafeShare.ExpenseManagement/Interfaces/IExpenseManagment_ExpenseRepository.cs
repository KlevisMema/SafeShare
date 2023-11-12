using SafeShare.DataTransormObject.Expenses;
using SafeShare.Utilities.Responses;

namespace SafeShare.ExpenseManagement.Interfaces
{
    public interface IExpenseManagment_ExpenseRepository
    {
        Task<Util_GenericResponse<IEnumerable<DTO_Expense>>> GetAllExpensesForGroup(Guid groupId, string userId);
        Task<Util_GenericResponse<DTO_Expense>> GetExpense(Guid expenseId, string userId);
        Task<Util_GenericResponse<DTO_Expense>> CreateExpense(DTO_Expense expenseDto, string userId);
        Task<Util_GenericResponse<bool>> DeleteExpense(Guid expenseId, string userId);
        Task<Util_GenericResponse<DTO_Expense>> EditExpense(Guid expenseId, DTO_ExpenseCreate expenseCreateDto, string userId);
    }
}