using AutoMapper;
using System.Net;
using System.Text;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SafeShare.Utilities.Responses;
using Microsoft.EntityFrameworkCore;
using SafeShare.Utilities.Dependencies;
using SafeShare.DataAccessLayer.Models;
using SafeShare.DataAccessLayer.Context;
using SafeShare.DataTransormObject.Expenses;
using SafeShare.ExpenseManagement.Interfaces;

namespace SafeShare.ExpenseManagement.Implementations;

public class ExpenseManagment_ExpenseRepository :
    Util_BaseContextDependencies<ApplicationDbContext, ExpenseManagment_ExpenseRepository>, 
    IExpenseManagment_ExpenseRepository
{
    public ExpenseManagment_ExpenseRepository
    (
        ApplicationDbContext db,
        IMapper mapper,
        ILogger<ExpenseManagment_ExpenseRepository> logger,
        IHttpContextAccessor httpContextAccessor
    )
    : base
    (
        db,
        mapper,
        logger,
        httpContextAccessor
    )
    { }

    public async Task<Util_GenericResponse<IEnumerable<DTO_Expense>>>
    GetAllExpensesForGroup
    (
        Guid groupId,
        string userId
    )
    {
        try
        {
            var isGroupMember = await _db.GroupMembers.AnyAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (!isGroupMember)
            {
                return Util_GenericResponse<IEnumerable<DTO_Expense>>.Response
                (
                    null,
                    false,
                    "User is not a member of the group.",
                    new List<string> { "Unauthorized" },
                    HttpStatusCode.Unauthorized
                );
            }

            var expenses = await _db.Expenses
                .Where(e => e.GroupId == groupId)
                .ToListAsync();

            var expenseDtos = expenses.Select(expense => new DTO_Expense
            {
                Id = expense.Id,
                Title = expense.Title,
                Date = expense.Date,
                Amount = expense.Amount,
                Description = expense.Desc,
                GroupId = expense.GroupId
            }).ToList();

            return Util_GenericResponse<IEnumerable<DTO_Expense>>.Response
            (
                expenseDtos,
                true,
                "Expenses retrieved successfully.",
                null,
                HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {
            return Util_GenericResponse<IEnumerable<DTO_Expense>>.Response
            (
                null,
                false,
                "An error occurred while retrieving expenses.",
                new List<string> { ex.Message },
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<Util_GenericResponse<DTO_Expense>>
    CreateExpense
    (
        DTO_Expense expenseDto,
        string userId
    )
    {
        try
        {
            var isMember = _db.GroupMembers.Any(gm => gm.GroupId == expenseDto.GroupId && gm.UserId == userId);

            if (!isMember)
            {
                return Util_GenericResponse<DTO_Expense>.Response
                (
                    null,
                    false,
                    "User is not part of the group.",
                    new List<string> { "Access Denied" },
                    HttpStatusCode.Forbidden
                );
            }

            var expense = new Expense
            {
                Title = expenseDto.Title,
                Date = expenseDto.Date,
                Amount = expenseDto.Amount,
                Desc = expenseDto.Description,
                GroupId = expenseDto.GroupId
            };

            _db.Expenses.Add(expense);
            await _db.SaveChangesAsync();

            return Util_GenericResponse<DTO_Expense>.Response
            (
                expenseDto,
                true,
                "Expense created successfully.",
                null,
                HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {
            return Util_GenericResponse<DTO_Expense>.Response
            (
                null,
                false,
                "An error occurred while creating the expense.",
                new List<string> { ex.ToString() },
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<Util_GenericResponse<DTO_Expense>>
    GetExpense
    (
        Guid expenseId,
        string userId
    )
    {
        try
        {
            var expense = await _db.Expenses
                .Include(e => e.ExpenseMembers)
                .FirstOrDefaultAsync(e => e.Id == expenseId);

            if (expense == null || !expense.ExpenseMembers.Any(em => em.UserId == userId))
            {
                return Util_GenericResponse<DTO_Expense>.Response(null, false, "Expense not found or access denied.", new List<string> { "Expense not found or access denied." }, HttpStatusCode.NotFound);
            }

            var expenseDto = new DTO_Expense
            {
                Id = expense.Id,
                Title = expense.Title,
                Date = expense.Date,
                Amount = expense.Amount,
                Description = expense.Desc
            };

            return Util_GenericResponse<DTO_Expense>.Response
            (
                expenseDto,
                true,
                "Expense retrieved successfully.",
                null,
                HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {
            return Util_GenericResponse<DTO_Expense>.Response
            (
                null,
                false,
                "An error occurred while retrieving the expense.",
                new List<string> { ex.ToString() },
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<Util_GenericResponse<DTO_Expense>>
    EditExpense
    (
        Guid expenseId,
        DTO_ExpenseCreate expenseCreateDto,
        string userId
    )
    {
        try
        {
            var expense = await _db.ExpenseMembers.Include(e => e.Expense).FirstOrDefaultAsync(e => e.ExpenseId == expenseId);

            if (expense == null || expense.UserId != userId)
            {
                return Util_GenericResponse<DTO_Expense>.Response
                (
                    null,
                    false,
                    "Expense not found or user not authorized to edit.",
                    new List<string> { "Unauthorized or not found." },
                    HttpStatusCode.Unauthorized
                );
            }

            expense.Expense.Title = expenseCreateDto.Title;
            expense.Expense.Date = expenseCreateDto.Date;
            expense.Expense.Amount = expenseCreateDto.Amount;
            expense.Expense.Desc = expenseCreateDto.Description;

            _db.Expenses.Update(expense.Expense);
            await _db.SaveChangesAsync();

            var updatedDTO_Expense = new DTO_Expense
            {
                Id = expense.Expense.Id,
                Title = expense.Expense.Title,
                Date = expense.Expense.Date,
                Amount = expense.Expense.Amount,
                Description = expense.Expense.Desc,
                GroupId = expense.Expense.GroupId
            };

            return Util_GenericResponse<DTO_Expense>.Response
            (
                updatedDTO_Expense,
                true,
                "Expense edited successfully.",
                null,
                HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {
            return Util_GenericResponse<DTO_Expense>.Response
            (
                null,
                false,
                "An error occurred while editing the expense.",
                new List<string> { ex.Message },
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<Util_GenericResponse<bool>>
    DeleteExpense
    (
        Guid expenseId,
        string userId
    )
    {
        try
        {
            var expense = await _db.Expenses.FindAsync(expenseId);

            if (expense == null || !expense.ExpenseMembers.Any(em => em.UserId == userId && em.UserId == userId))
            {
                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    "Expense not found or user is not authorized to delete this expense.",
                    new List<string> { "Not found or not authorized." },
                    HttpStatusCode.NotFound
                );
            }

            _db.Expenses.Remove(expense);
            await _db.SaveChangesAsync();

            return Util_GenericResponse<bool>.Response
            (
                true,
                true,
                "Expense deleted successfully.",
                null,
                HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {
            return Util_GenericResponse<bool>.Response
            (
                false,
                false,
                "An error occurred while deleting the expense.",
                new List<string> { ex.ToString() },
                HttpStatusCode.InternalServerError
            );
        }
    }
}