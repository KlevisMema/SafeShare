using SafeShare.ClientDTO.Expense;
using SafeShare.ClientUtilities.Responses;

namespace SafeShare.ClientServices.Interfaces
{
    public interface IClientService_ExpenseManagment
    {
        Task<ClientUtil_ApiResponse<ClientDto_Expense>> CreateExpense(ClientDto_ExpenseCreate clientDto_ExpenseCreate);
        Task<ClientUtil_ApiResponse<bool>> DeleteExpense(ClientDto_ExpenseDelete clientDto_ExpenseDelete);
        Task<ClientUtil_ApiResponse<ClientDto_Expense>> EditExpense(Guid expenseId, ClientDto_ExpenseCreate clientDto_ExpenseCreate);
        Task<ClientUtil_ApiResponse<List<ClientDto_Expense>>> GetAllExpensesOfGroup(Guid groupId);
        Task<ClientUtil_ApiResponse<ClientDto_Expense>> GetExpense(Guid groupId);
    }
}