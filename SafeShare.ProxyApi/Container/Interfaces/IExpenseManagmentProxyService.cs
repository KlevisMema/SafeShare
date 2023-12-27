using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.DataTransormObject.SafeShareApi.ExpenseManagment;

namespace SafeShare.ProxyApi.Container.Interfaces;

public interface IExpenseManagmentProxyService
{
    Task<Util_GenericResponse<List<DTO_Expense>>>
    GetAllExpensesOfGroup
    (
        string userId,
        string jwtToken,
        Guid groupId
    );

    Task<Util_GenericResponse<DTO_Expense>>
    GetExpense
    (
        string userId,
        string jwtToken,
        Guid expenseId
    );

    Task<Util_GenericResponse<DTO_Expense>>
    CreateExpense
    (
        string userId,
        string jwtToken,
        DTO_ExpenseCreate expenseDto
    );

    Task<Util_GenericResponse<DTO_Expense>>
    EditExpense
    (
        string userId,
        string jwtToken,
        Guid expenseId,
        DTO_ExpenseCreate expenseCreateDto
    );

    Task<Util_GenericResponse<bool>>
    DeleteExpense
    (
        string userId,
        string jwtToken,
        DTO_ExpenseDelete expenseDelete
    );
}