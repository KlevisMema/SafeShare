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
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using SafeShare.Security.GroupSecurity;
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
    IHttpContextAccessor httpContextAccessor,
    IGroupKeySecurity groupKeySecurity
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
            var isGroupMember = await _db.GroupMembers.AnyAsync(gm => gm.GroupId == groupId && gm.UserId == userId && !gm.User.IsDeleted && !gm.IsDeleted && !gm.Group.IsDeleted);

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
                    ["Access Denied"],
                    HttpStatusCode.Forbidden
                );
            }

            CancellationTokenSource cts = new();
            CancellationToken cancellationToken = cts.Token;

            List<string> groupMembersIds = await _db.GroupMembers.Where(x => x.GroupId == groupId).Select(x => x.UserId).ToListAsync();

            var expenses = await _db.Expenses
                .Include(x => x.ExpenseMembers)
                .ThenInclude(x => x.User)
                .Where(e => e.GroupId == groupId && !e.Group.IsDeleted && !e.IsDeleted)
                .ToListAsync();

            var getDecryptedExpenses = await DecryptMultipleExpensesData(groupId, groupMembersIds, expenses, cancellationToken);

            if (!getDecryptedExpenses.Succsess || getDecryptedExpenses.Value is null)
            {
                return Util_GenericResponse<List<DTO_Expense>>.Response
                (
                    null,
                    false,
                    "Expenses retrieved unsuccessfully.",
                    null,
                    HttpStatusCode.BadRequest
                );
            }

            var mappedExpenseToDto = getDecryptedExpenses.Value.Select(e => new DTO_Expense
            {
                Id = e.Id,
                Title = e.Title,
                Date = e.Date,
                Amount = e.Amount,
                Description = e.Desc,
                GroupId = e.GroupId,
                CreatedByMe = e.ExpenseMembers.Any(em => em.UserId == userId && em.CreatorOfExpense),
                CreatorOfExpense = e.ExpenseMembers
                        .Where(em => em.CreatorOfExpense)
                        .Select(em => em.User.FullName)
                        .FirstOrDefault() ?? string.Empty
            }).ToList();

            return Util_GenericResponse<List<DTO_Expense>>.Response
            (
                mappedExpenseToDto,
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
                .FirstOrDefaultAsync(e => e.Id == expenseId && !e.IsDeleted);

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

            var getExpenseDecrypted = await DecryptExpenseData(userId, expense);

            if (!getExpenseDecrypted.Succsess || getExpenseDecrypted.Value is null)
            {
                return Util_GenericResponse<DTO_Expense>.Response
                (
                    null,
                    false,
                    "Expense was not retrieved successfully.",
                    null,
                    HttpStatusCode.BadRequest
                );
            }

            var mappedDtoExpense = _mapper.Map<DTO_Expense>(getExpenseDecrypted.Value);

            mappedDtoExpense.CreatedByMe = expense.ExpenseMembers.Any(em => em.UserId == userId && em.CreatorOfExpense);
            mappedDtoExpense.CreatorOfExpense = expense.ExpenseMembers
                       .Where(em => em.UserId == userId && em.CreatorOfExpense)
                       .Select(em => em.User.FullName)
                       .FirstOrDefault() ?? string.Empty;

            return Util_GenericResponse<DTO_Expense>.Response
            (
                mappedDtoExpense,
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
                                                     .ThenInclude(x => x.User)
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
                    ["Access Denied"],
                    HttpStatusCode.Forbidden
                );
            }

            var canParseAmount = int.TryParse(expenseDto.Amount, out int amountParsed);

            if (!canParseAmount)
            {
                return Util_GenericResponse<DTO_Expense>.Response
                (
                    null,
                    false,
                    "Expense was not created. Invalid amount of money",
                    null,
                    HttpStatusCode.BadRequest
                );
            }

            var cpyOfDto = new DTO_Expense
            {
                Amount = expenseDto.Amount,
                Title = expenseDto.Title,
                Description = expenseDto.Description,
                Date = expenseDto.Date,
                GroupId = expenseDto.GroupId,
                CreatedByMe = true,
                CreatorOfExpense = isMember.User.FullName
            };

            var resultEncryption = await EncryptExpenseData(userId, expenseDto);

            if (!resultEncryption.Succsess || resultEncryption.Value is null)
            {
                return Util_GenericResponse<DTO_Expense>.Response
                (
                    null,
                    false,
                    "Expense was not created.",
                    null,
                    HttpStatusCode.BadRequest
                );
            }

            expenseDto = resultEncryption.Value;

            var expense = _mapper.Map<Expense>(expenseDto);

            _db.Expenses.Add(expense);

            int memberCount = groupMembers.Count;

            if (memberCount <= 1)
            {
                return Util_GenericResponse<DTO_Expense>.Response
                (
                    null,
                    false,
                    "Can not create an expense if no other members are in the group",
                    null,
                    HttpStatusCode.BadRequest
                );
            }

            decimal shareAmount = amountParsed / (memberCount - 1);

            foreach (var member in groupMembers)
            {
                if (member.UserId == userId)
                    member.Balance += (amountParsed - shareAmount);
                else
                    member.Balance -= shareAmount;

                _db.GroupMembers.Update(member);
            }

            var expenseMembers = new List<ExpenseMember>();

            foreach (var member in groupMembers)
            {
                expenseMembers.Add(new ExpenseMember
                {
                    CreatedAt = member.CreatedAt,
                    CreatorOfExpense = member.UserId == userId,
                    DeletedAt = null,
                    ExpenseId = expense.Id,
                    IsDeleted = false,
                    UserId = member.UserId,
                    ModifiedAt = null,
                });
            }

            await _db.ExpenseMembers.AddRangeAsync(expenseMembers);

            await _db.SaveChangesAsync();

            await transaction.CommitAsync();

            cpyOfDto.Id = expense.Id;

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
                cpyOfDto,
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
        DTO_ExpenseCreate expenseEdit,
        string userId
    )
    {
        using var transaction = _db.Database.BeginTransaction();

        try
        {
            var groupMembers = await _db.GroupMembers.Include(x => x.Group)
                                                   .ThenInclude(x => x.GroupMembers)
                                                   .Where(gm => gm.GroupId == expenseEdit.GroupId)
                                                   .ToListAsync();


            var isMember = groupMembers.FirstOrDefault(gm => gm.UserId == userId);

            if (isMember is null || isMember.IsDeleted)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[EditExpense Method] => 
                        [RESULT] : [IP] {IP}, user with [ID] {ID} is not a memeber of the group with [ID] {groupId}.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    userId,
                    expenseEdit.GroupId
                );

                return Util_GenericResponse<DTO_Expense>.Response
                (
                    null,
                    false,
                    "User is not part of the group.",
                    ["Access Denied"],
                    HttpStatusCode.Forbidden
                );
            }


            var expense = await _db.Expenses.Include(em => em.ExpenseMembers)
                                                  .ThenInclude(e => e.User)
                                                  .FirstOrDefaultAsync(e => e.Id == expenseId && !e.IsDeleted && e.GroupId == expenseEdit.GroupId);

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

            var expenseCreator = expense.ExpenseMembers.FirstOrDefault(em => em.UserId == userId);

            if (expenseCreator is null || expenseCreator.IsDeleted || !expenseCreator.CreatorOfExpense || expenseCreator.UserId != userId)
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

            var canParseAmount = int.TryParse(expenseEdit.Amount, out int amountParsed);

            if (!canParseAmount)
            {
                return Util_GenericResponse<DTO_Expense>.Response
                (
                    null,
                    false,
                    "Expense was not edited. Invalid amount of money",
                    null,
                    HttpStatusCode.BadRequest
                );
            }

            var cpyOfDto = new DTO_Expense
            {
                Amount = expenseEdit.Amount,
                Title = expenseEdit.Title,
                Description = expenseEdit.Description,
                Date = expenseEdit.Date,
                GroupId = expenseEdit.GroupId,
                CreatedByMe = true,
                CreatorOfExpense = isMember.User.FullName,
                Id = expenseId
            };

            var resultEncryption = await EncryptExpenseData(userId, expenseEdit);

            if (!resultEncryption.Succsess || resultEncryption.Value is null)
            {
                return Util_GenericResponse<DTO_Expense>.Response
                (
                    null,
                    false,
                    "Expense was not edited.",
                    null,
                    HttpStatusCode.BadRequest
                );
            }

            bool tryParseDate = DateTime.TryParse(cpyOfDto.Date, out DateTime parsedDate);

            if (!tryParseDate)
            {
                return Util_GenericResponse<DTO_Expense>.Response
                (
                    null,
                    false,
                    "Expense was not edited.",
                    null,
                    HttpStatusCode.BadRequest
                );
            }

            expense.ModifiedAt = DateTime.Parse(cpyOfDto.Date);
            expense.Desc = resultEncryption.Value.Description;
            expense.Title = resultEncryption.Value.Title;
            expense.Amount = resultEncryption.Value.Amount;

            _db.Expenses.Update(expense);

            int memberCount = groupMembers.Count;

            if (memberCount <= 1)
            {
                return Util_GenericResponse<DTO_Expense>.Response
                (
                    null,
                    false,
                    "Can not edit an expense if no other members are in the group",
                    null,
                    HttpStatusCode.BadRequest
                );
            }

            decimal shareAmount = amountParsed / (memberCount - 1);

            foreach (var member in groupMembers)
            {
                if (member.UserId == userId)
                    member.Balance += (amountParsed - shareAmount);
                else
                    member.Balance -= shareAmount;

                _db.GroupMembers.Update(member);
            }

            var expenseMembers = expense.ExpenseMembers;

            foreach (var member in expenseMembers)
                member.ModifiedAt = DateTime.UtcNow;

            _db.ExpenseMembers.UpdateRange(expenseMembers);

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
                cpyOfDto,
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
            var groupMembers = await _db.GroupMembers.Include(x => x.Group)
                                                   .ThenInclude(x => x.GroupMembers)
                                                   .Where(gm => gm.GroupId == expenseDelete.GroupId)
                                                   .ToListAsync();


            var isMember = groupMembers.FirstOrDefault(gm => gm.UserId == expenseDelete.UserId.ToString());

            if (isMember is null || isMember.IsDeleted)
            {
                await transaction.DisposeAsync();

                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[DeleteExpense Method] => 
                        [RESULT] : [IP] {IP}, user with [ID] {ID} is not a memeber of the group with [ID] {groupId}.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    expenseDelete.UserId,
                    expenseDelete.GroupId
                );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    "User is not part of the group.",
                    ["Access Denied"],
                    HttpStatusCode.Forbidden
                );
            }

            var expense = await _db.Expenses.Include(x => x.Group)
                                            .ThenInclude(x => x.GroupMembers)
                                            .ThenInclude(x => x.User)
                                            .ThenInclude(x => x.ExpenseMembers)
                                            .FirstOrDefaultAsync(ex => ex.Id == expenseDelete.ExpenseId && ex.GroupId == expenseDelete.GroupId);

            if (expense is null || expense.IsDeleted)
            {
                await transaction.DisposeAsync();

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

            var expenseMadeByCreator = expense.ExpenseMembers.Any(em => em.UserId == expenseDelete.UserId.ToString() && !em.IsDeleted && !em.User.IsDeleted);

            if (!expenseMadeByCreator)
            {
                await transaction.RollbackAsync();

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

            var getExpenseDecrypted = await DecryptExpenseData(expenseDelete.UserId.ToString(), expense);

            if (getExpenseDecrypted is null || !getExpenseDecrypted.Succsess || getExpenseDecrypted.Value is null)
            {
                await transaction.DisposeAsync();

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    "Expense was not deleted.",
                    null,
                    HttpStatusCode.BadRequest
                );
            }

            expense.IsDeleted = true;

            var shareAmount = expenseDelete.ExpenseAmount / (groupMembers.Count - 1);

            foreach (var member in groupMembers)
            {
                if (member.UserId == expenseDelete.UserId.ToString())
                    member.Balance -= (decimal.Parse(expense.Amount) - shareAmount);
                else
                    member.Balance += shareAmount;

                _db.GroupMembers.Update(member);
            }

            foreach (var member in expense.ExpenseMembers)
                _db.ExpenseMembers.Remove(member);

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

    private async Task<Util_GenericResponse<DTO_ExpenseCreate>>
    EncryptExpenseData
    (
        string userId,
        DTO_ExpenseCreate expense
    )
    {
        try
        {
            byte[]? userKey = await groupKeySecurity.DeriveUserKey(iterations: 10000, outputLength: 32, userId, expense.GroupId);

            if (userKey is null)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[EncryptExpenseData Method] => 
                        [RESULT] : User key was not generated.
                     """
                );

                return Util_GenericResponse<DTO_ExpenseCreate>.Response
                (
                    null,
                    false,
                    string.Empty,
                    null,
                    HttpStatusCode.InternalServerError
                );
            }

            expense.Title = EncryptData(expense.Title, userKey);
            expense.Date = EncryptData(expense.Date.ToString(), userKey);
            expense.Amount = EncryptData(expense.Amount, userKey);
            expense.Description = EncryptData(expense.Description, userKey);

            return Util_GenericResponse<DTO_ExpenseCreate>.Response
            (
                expense,
                true,
                string.Empty,
                null,
                HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {
            _logger.Log
            (
                LogLevel.Critical,
                """
                    [ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[EncryptExpenseData Method] => 
                    [RESULT] : User key was not generated. An exception occurred  => {ex}
                 """,
                ex
            );

            return Util_GenericResponse<DTO_ExpenseCreate>.Response
            (
                null,
                false,
                string.Empty,
                null,
                HttpStatusCode.InternalServerError
            );
        }
    }

    private async Task<Util_GenericResponse<List<Expense>>>
    DecryptMultipleExpensesData
    (
        Guid groupId,
        List<string> userIds,
        List<Expense> expenses,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            List<Expense> decryptedExpenses = [];

            foreach (var userId in userIds)
            {
                byte[]? userKey = await groupKeySecurity.DeriveUserKey(iterations: 10000, outputLength: 32, userId, groupId);

                foreach (var expense in expenses)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        var decryptedExpense = new Expense
                        {
                            Id = expense.Id,
                            Title = DecryptData(expense.Title, userKey),
                            Date = DecryptData(expense.Date, userKey),
                            Amount = DecryptData(expense.Amount, userKey),
                            Desc = DecryptData(expense.Desc, userKey),
                            GroupId = expense.GroupId,
                            Group = expense.Group,
                            ExpenseMembers = expense.ExpenseMembers,
                            DeletedAt = expense.DeletedAt,
                            CreatedAt = expense.CreatedAt,
                            IsDeleted = expense.IsDeleted,
                            ModifiedAt = expense.ModifiedAt,
                        };
                        decryptedExpenses.Add(decryptedExpense);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                }
            }

            return Util_GenericResponse<List<Expense>>.Response
            (
                decryptedExpenses,
                true,
                null,
                null,
                HttpStatusCode.OK
            );

        }
        catch (OperationCanceledException)
        {
            _logger.Log
            (
                LogLevel.Error,
                "[ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[DecryptExpenseDataAsync Method] => Operation was canceled."
            );

            return Util_GenericResponse<List<Expense>>.Response(
                null,
                false,
                "Operation was canceled.",
                null,
                HttpStatusCode.BadRequest
            );
        }
        catch (Exception ex)
        {
            _logger.Log
            (
                LogLevel.Critical,
                """
                    [ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[DecryptExpenseData Method] => 
                    [RESULT] : Expense was not encrypted {ex}
                 """,
                ex
            );

            return Util_GenericResponse<List<Expense>>.Response
            (
                null,
                false,
                string.Empty,
                null,
                HttpStatusCode.InternalServerError
            );
        }
    }

    private async Task<Util_GenericResponse<Expense>>
    DecryptExpenseData
    (
        string userId,
        Expense expense,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            byte[]? userKey = await groupKeySecurity.DeriveUserKey(iterations: 10000, outputLength: 32, userId, expense.GroupId);

            expense.Title = DecryptData(expense.Title, userKey);
            expense.Date = DecryptData(expense.Date, userKey);
            expense.Amount = DecryptData(expense.Amount, userKey);
            expense.Desc = DecryptData(expense.Desc, userKey);

            return Util_GenericResponse<Expense>.Response
            (
                expense,
                true,
                string.Empty,
                null,
                HttpStatusCode.OK
            );

        }
        catch (OperationCanceledException)
        {
            _logger.Log
            (
                LogLevel.Error,
                "[ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[DecryptExpenseDataAsync Method] => Operation was canceled."
            );

            return Util_GenericResponse<Expense>.Response(
                null,
                false,
                "Operation was canceled.",
                null,
                HttpStatusCode.BadRequest
            );
        }
        catch (Exception ex)
        {
            _logger.Log
            (
                LogLevel.Critical,
                """
                    [ExpenseManagment Module]-[ExpenseManagment_ExpenseRepository class]-[DecryptExpenseData Method] => 
                    [RESULT] : Expense was not encrypted {ex}
                 """,
                ex
            );

            return Util_GenericResponse<Expense>.Response
            (
                null,
                false,
                string.Empty,
                null,
                HttpStatusCode.InternalServerError
            );
        }
    }

    private static string
    EncryptData
    (
        string data,
        byte[] key
    )
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);

#pragma warning disable SYSLIB0053
        using AesGcm aesGcm = new(key);
        byte[] nonce = new byte[12];
        byte[] ciphertext = new byte[dataBytes.Length];
        byte[] tag = new byte[16];
        aesGcm.Encrypt(nonce, dataBytes, ciphertext, tag, null);
#pragma warning restore SYSLIB0053

        byte[] encryptedDataWithTag = new byte[nonce.Length + ciphertext.Length];
        Buffer.BlockCopy(nonce, 0, encryptedDataWithTag, 0, nonce.Length);
        Buffer.BlockCopy(ciphertext, 0, encryptedDataWithTag, nonce.Length, ciphertext.Length);

        encryptedDataWithTag = [.. encryptedDataWithTag, .. tag];

        string base64String = Convert.ToBase64String(encryptedDataWithTag);

        return base64String;
    }

    private static string
    DecryptData
    (
        string encryptedDataWithTag,
        byte[] key
    )
    {
        byte[] encryptedDataWithTagBytes = Convert.FromBase64String(encryptedDataWithTag);

#pragma warning disable SYSLIB0053
        using AesGcm aesGcm = new(key);
        byte[] nonce = new byte[12];
        Buffer.BlockCopy(encryptedDataWithTagBytes, 0, nonce, 0, nonce.Length);

        byte[] ciphertext = new byte[encryptedDataWithTagBytes.Length - nonce.Length - 16];
        Buffer.BlockCopy(encryptedDataWithTagBytes, nonce.Length, ciphertext, 0, ciphertext.Length);

        byte[] tag = new byte[16];
        Buffer.BlockCopy(encryptedDataWithTagBytes, nonce.Length + ciphertext.Length, tag, 0, tag.Length);

        aesGcm.Decrypt(nonce, ciphertext, tag, ciphertext, null);

        return Encoding.UTF8.GetString(ciphertext);
#pragma warning restore SYSLIB0053
    }
}