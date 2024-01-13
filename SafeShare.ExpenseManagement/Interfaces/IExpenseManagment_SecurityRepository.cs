using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.DataAccessLayer.Models.SafeShareApi;
using SafeShare.DataTransormObject.SafeShareApi.ExpenseManagment;

namespace SafeShare.ExpenseManagement.Interfaces;

public interface IExpenseManagment_SecurityRepository
{
    Task<Util_GenericResponse<DTO_ExpenseCreate>>
    EncryptExpenseData
    (
        string userId,
        DTO_ExpenseCreate expense,
        Guid tag
    );

    Task<Util_GenericResponse<Expense>>
    DecryptExpenseData
    (
        string userId,
        Expense expense,
        Guid tag,
        CancellationToken cancellationToken = default
    );

    Task<Util_GenericResponse<List<Expense>>>
    DecryptMultipleExpensesData
    (
        Guid groupId,
        List<string> userIds,
        List<Expense> expenses,
        Guid tag,
        CancellationToken cancellationToken = default
    );
}