/*
 * This file contains the implementation of the Expense Management Repository which includes CRUD operations
 * for expenses within a group context. It utilizes a base repository for common dependencies and provides
 * functionality to manage expense-related data, ensuring that user balances are properly updated according
 * to the group's shared expenses.
 */

using AutoMapper;
using System.Net;
using System.Text;
using AutoMapper.Execution;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using SafeShare.DataAccessLayer.Context;
using SafeShare.Utilities.SafeShareApi.IP;
using SafeShare.Utilities.SafeShareApi.Log;
using SafeShare.ExpenseManagement.Interfaces;
using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.Utilities.SafeShareApi.Dependencies;
using SafeShare.DataAccessLayer.Models.SafeShareApi;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment;
using SafeShare.DataTransormObject.SafeShareApi.ExpenseManagment;

namespace SafeShare.ExpenseManagement.Implementations;

/// <summary>
/// Provides methods for managing expenses within the application's groups, 
/// including creating, retrieving, updating, and deleting expenses.
/// </summary>
/// <remarks>
/// Initializes a new instance of the ExpenseManagment_ExpenseRepository class with injected dependencies.
/// </remarks>
/// <param name="db">Database context for accessing the application's data.</param>
/// <param name="mapper">Automapper instance for mapping between entities and DTOs.</param>
/// <param name="logger">Logger instance for logging messages.</param>
/// <param name="httpContextAccessor">HTTP context accessor for accessing the current HTTP context.</param>
public class ExpenseManagment_ExpenseRepository
(
    ApplicationDbContext db,
    IMapper mapper,
    ILogger<ExpenseManagment_ExpenseRepository> logger,
    IHttpContextAccessor httpContextAccessor
) : Util_BaseContextDependencies<ApplicationDbContext, ExpenseManagment_ExpenseRepository>(
    db,
    mapper,
    logger,
    httpContextAccessor
), IExpenseManagment_ExpenseRepository
{
    /// <summary>
    /// Retrieves all expenses for a specified group that the user is a member of.
    /// </summary>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A generic response containing the list of expenses or an error message.</returns>
    public async Task<Util_GenericResponse<List<DTO_Expense>>>
    GetAllExpensesOfGroup
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
                _logger.Log
                (
                   LogLevel.Error,
                   """
                        [ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[GetAllExpensesForGroup Method] => 
                        [RESULT] : [IP] {IP} user with [ID] {ID} is not a memeber of the group with [ID] {groupId}.
                    """,
                   await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                   userId,
                   groupId
                );

                return Util_GenericResponse<List<DTO_Expense>>.Response
                (
                    null,
                    false,
                    "User is not a member of the group.",
                    new List<string> { "Access Denied" },
                    HttpStatusCode.Forbidden
                );
            }

            var expenses = await _db.Expenses
                .Where(e => e.GroupId == groupId)
                .ToListAsync();

            return Util_GenericResponse<List<DTO_Expense>>.Response
            (
                expenses.Select(_mapper.Map<DTO_Expense>).ToList(),
                true,
                "Expenses retrieved successfully.",
                null,
                HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<List<DTO_Expense>, ExpenseManagment_ExpenseRepository>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                    Somewthing went wrong in [ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[GetAllExpensesForGroup Method],
                    user with [ID] {userId} tried to get all the expenses of the group with [ID] {groupId}.
                 """,
                null,
                _httpContextAccessor
            );
        }
    }
    /// <summary>
    /// Retrieves a specific expense by its ID, provided the user has access to it.
    /// </summary>
    /// <param name="expenseId">The unique identifier of the expense.</param>
    /// <param name="userId">The unique identifier of the user requesting the expense.</param>
    /// <returns>A generic response containing the expense or an error message.</returns>
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

            if (expense == null || expense.IsDeleted)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[GetExpense Method] => 
                        [RESULT] : [IP] {IP} expense with [ID] {ID} does not exists.
                        """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    expenseId
                );

                return Util_GenericResponse<DTO_Expense>.Response
                (
                    null,
                    false,
                    "Expense not found",
                    null,
                    HttpStatusCode.NotFound
                );
            }

            if (!expense.ExpenseMembers.Any(em => em.UserId == userId))
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[GetExpense Method] => 
                        [RESULT] : [IP] {IP} user with [ID] {ID} does not have access for the expsense with [ID] {expenseId}.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    userId,
                    expenseId
                );

                return Util_GenericResponse<DTO_Expense>.Response
                (
                    null,
                    false,
                    "Access denied.",
                    null,
                    HttpStatusCode.Forbidden
                );
            }

            return Util_GenericResponse<DTO_Expense>.Response
            (
                _mapper.Map<DTO_Expense>(expense),
                true,
                "Expense retrieved successfully.",
                null,
                HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<DTO_Expense, ExpenseManagment_ExpenseRepository>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                    Somewthing went wrong in [ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[GetExpense Method],
                    user with [ID] {userId} tried to get the expense with [ID] {expenseId}.
                 """,
                null,
                _httpContextAccessor
            );
        }
    }
    /// <summary>
    /// Creates a new expense within a group and updates the balances of all group members.
    /// </summary>
    /// <param name="expenseDto">Data transfer object containing the expense details to be created.</param>
    /// <param name="userId">The unique identifier of the user creating the expense.</param>
    /// <returns>A generic response indicating the result of the create operation.</returns>
    public async Task<Util_GenericResponse<DTO_Expense>>
    CreateExpense
    (
        DTO_ExpenseCreate expenseDto,
        string userId
    )
    {
        using var transaction = _db.Database.BeginTransaction();

        try
        {
            var groupMembers = await _db.GroupMembers.Include(x => x.Group)
                                                     .ThenInclude(x => x.GroupMembers)
                                                     .Where(gm => gm.GroupId == expenseDto.GroupId)
                                                     .ToListAsync();


            var isMember = groupMembers.FirstOrDefault(gm => gm.UserId == userId);

            if (isMember is null || isMember.IsDeleted)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[CreateExpense Method] => 
                        [RESULT] : [IP] {IP}, user with [ID] {ID} is not a memeber of the group with [ID] {groupId}.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    userId,
                    expenseDto.GroupId
                );

                return Util_GenericResponse<DTO_Expense>.Response
                (
                    null,
                    false,
                    "User is not part of the group.",
                    new List<string> { "Access Denied" },
                    HttpStatusCode.Forbidden
                );
            }

            var expense = _mapper.Map<Expense>(expenseDto);

            _db.Expenses.Add(expense);

            int memberCount = groupMembers.Count;
            decimal shareAmount = expenseDto.DecryptedAmount / (memberCount - 1);

            foreach (var member in groupMembers)
            {
                if (member.UserId == userId)
                    member.Balance += (expenseDto.DecryptedAmount - shareAmount);
                else
                    member.Balance -= shareAmount;

                _db.GroupMembers.Update(member);
            }

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.Log
            (
                LogLevel.Information,
                """
                    [ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[CreateExpense Method] => 
                    [RESULT] : [IP] {IP}, user with [ID] {ID} created an expense in the group with [ID] {groupId}.
                    The id of the 
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                userId,
                expenseDto.GroupId
            );

            return Util_GenericResponse<DTO_Expense>.Response
            (
                _mapper.Map<DTO_Expense>(expenseDto),
                true,
                "Expense created successfully.",
                null,
                HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            return await Util_LogsHelper<DTO_Expense, ExpenseManagment_ExpenseRepository>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                    Somewthing went wrong in [ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[CreateExpense Method],
                    user with [ID] {userId} tried to create an expense in the group with [ID] {expenseDto.GroupId}.
                 """,
                null,
                _httpContextAccessor
            );
        }
    }
    /// <summary>
    /// Updates an existing expense and adjusts group members' balances accordingly.
    /// </summary>
    /// <param name="expenseId">The unique identifier of the expense to be updated.</param>
    /// <param name="expenseCreateDto">Data transfer object containing the updated expense details.</param>
    /// <param name="userId">The unique identifier of the user editing the expense.</param>
    /// <returns>A generic response indicating the result of the update operation.</returns>
    public async Task<Util_GenericResponse<DTO_Expense>>
    EditExpense
    (
        Guid expenseId,
        DTO_ExpenseCreate expenseCreateDto,
        string userId
    )
    {
        using var transaction = _db.Database.BeginTransaction();

        try
        {
            var expense = await _db.ExpenseMembers.Include(e => e.Expense).FirstOrDefaultAsync(e => e.ExpenseId == expenseId);

            if (expense is null || expense.IsDeleted)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[EditExpense Method] => 
                        [RESULT] : [IP] {IP}, expense with [ID] {ID} doesn't exist.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    expenseId
                );

                return Util_GenericResponse<DTO_Expense>.Response
                (
                    null,
                    false,
                    "Expense not found.",
                    null,
                    HttpStatusCode.NotFound
                );
            }

            if (expense.UserId != userId)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[EditExpense Method] => 
                        [RESULT] : [IP] {IP}, user with [ID] {ID} is not the creator of this expense [ID] {expenseId}.
                        Therefore he can not edit it.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    userId,
                    expenseId
                );

                return Util_GenericResponse<DTO_Expense>.Response
                (
                    null,
                    false,
                    "Not authorized to edit.",
                    null,
                    HttpStatusCode.Forbidden
                );
            }

            expense.Expense.Title = expenseCreateDto.Title;
            expense.Expense.Date = expenseCreateDto.Date;
            expense.Expense.Amount = expenseCreateDto.Amount;
            expense.Expense.Desc = expenseCreateDto.Description;

            _db.Expenses.Update(expense.Expense);

            var members = await _db.GroupMembers.Where(m => m.GroupId == expense.Expense.GroupId).ToListAsync();

            foreach (var member in members)
            {
                if (member.UserId == userId)
                    member.Balance += expenseCreateDto.DecryptedAmount;
                else
                    member.Balance -= expenseCreateDto.DecryptedAmount / (members.Count - 1);

                _db.GroupMembers.Update(member);
            }

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.Log
            (
                LogLevel.Information,
                """
                    [ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[EditExpense Method] => 
                    [RESULT] : [IP] {IP}, user with [ID] {ID} edited expense [ID] {expenseId}.
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                userId,
                expenseId
            );

            return Util_GenericResponse<DTO_Expense>.Response
            (
                _mapper.Map<DTO_Expense>(expense.Expense),
                true,
                "Expense edited successfully.",
                null,
                HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            return await Util_LogsHelper<DTO_Expense, ExpenseManagment_ExpenseRepository>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                    Somewthing went wrong in [ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[EditExpense Method],
                    user with [ID] {userId} tried to edit an expense with [ID] {expenseId}.
                 """,
                null,
                _httpContextAccessor
            );
        }
    }
    /// <summary>
    /// Deletes an expense by marking it as deleted and adjusts the balances of all affected group members.
    /// </summary>
    /// <param name="expenseDelete">Data transfer object containing the details of the expense to be deleted.</param>
    /// <returns>A generic response indicating the result of the delete operation.</returns>
    public async Task<Util_GenericResponse<bool>>
    DeleteExpense
    (
        DTO_ExpenseDelete expenseDelete
    )
    {
        using var transaction = _db.Database.BeginTransaction();

        try
        {
            var expense = await _db.Expenses.Include(x => x.Group)
                                            .ThenInclude(x => x.GroupMembers)
                                            .FirstOrDefaultAsync(ex => ex.Id == expenseDelete.ExpenseId);

            if (expense is null || expense.IsDeleted)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[DeleteExpense Method] => 
                        [RESULT] : [IP] {IP}, expense with [ID] {ID} doesn't exist.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    expenseDelete.ExpenseId
                );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    "Expense not found",
                    null,
                    HttpStatusCode.NotFound
                );
            }

            var expenseMadeByCreator = expense.ExpenseMembers.Any(em => em.UserId == expenseDelete.UserId.ToString());

            if (!expenseMadeByCreator)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[DeleteExpense Method] => 
                        [RESULT] : [IP] {IP}, user with [ID] {ID} is not the creator of the expense [ID] {expenseId}.
                        Therefore he can not delete it.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    expenseDelete.UserId,
                    expenseDelete.ExpenseId
                );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    "User is not authorized to delete this expense.",
                    null,
                    HttpStatusCode.Forbidden
                );
            }

            expense.IsDeleted = true;

            var members = expense.Group.GroupMembers;
            var shareAmount = expenseDelete.ExpenseAmount / (members.Count - 1);

            foreach (var member in members)
            {
                if (member.UserId == expenseDelete.UserId.ToString())
                    member.Balance -= (expenseDelete.ExpenseAmount - shareAmount);
                else
                    member.Balance += shareAmount;

                _db.GroupMembers.Update(member);
            }

            _db.Expenses.Update(expense);

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.Log
            (
                LogLevel.Information,
                """
                    [ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[DeleteExpense Method] => 
                    [RESULT] : [IP] {IP}, user with [ID] {ID} successfully deleted expense [ID] {expenseId}.
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                expenseDelete.UserId,
                expenseDelete.ExpenseId
            );

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
            await transaction.RollbackAsync();

            return await Util_LogsHelper<bool, ExpenseManagment_ExpenseRepository>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                    Somewthing went wrong in [ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[DeleteExpense Method],
                    user with [ID] {expenseDelete.UserId} tried to delete an expense with [ID] {expenseDelete.ExpenseId}.
                 """,
                false,
                _httpContextAccessor
            );
        }
    }
}