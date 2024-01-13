using AutoMapper;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using SafeShare.Security.GroupSecurity;
using SafeShare.DataAccessLayer.Context;
using SafeShare.ExpenseManagement.Interfaces;
using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.Utilities.SafeShareApi.Dependencies;
using SafeShare.DataAccessLayer.Models.SafeShareApi;
using SafeShare.DataTransormObject.SafeShareApi.ExpenseManagment;

namespace SafeShare.ExpenseManagement.Implementations;

public class ExpenseManagment_SecurityRepository(
    ILogger<ExpenseManagment_SecurityRepository> logger,
    IGroupKeySecurity groupKeySecurity
) : Util_BaseContextDependencies<ApplicationDbContext, ExpenseManagment_SecurityRepository>(
    null!,
    null!,
    logger,
    null!
), IExpenseManagment_SecurityRepository
{
    public async Task<Util_GenericResponse<DTO_ExpenseCreate>>
    EncryptExpenseData
    (
        string userId,
        DTO_ExpenseCreate expense,
        Guid tag
    )
    {
        try
        {
            byte[]? userKey = await groupKeySecurity.DeriveUserKey(iterations: 10000, outputLength: 32, userId, expense.GroupId, tag);

            if (userKey is null)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [ExpenseManagment Module]-[ExpenseManagment_SecurityRepository class]-[EncryptExpenseData Method] => 
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
                    [ExpenseManagment Module]-[ExpenseManagment_SecurityRepository class]-[EncryptExpenseData Method] => 
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

    public async Task<Util_GenericResponse<List<Expense>>>
    DecryptMultipleExpensesData
    (
        Guid groupId,
        List<string> userIds,
        List<Expense> expenses,
        Guid tag,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            List<Expense> decryptedExpenses = [];

            foreach (var userId in userIds)
            {
                byte[]? userKey = await groupKeySecurity.DeriveUserKey(iterations: 10000, outputLength: 32, userId, groupId, tag);

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
                "[ExpenseManagment Module]-[ExpenseManagment_SecurityRepository class]-[DecryptExpenseDataAsync Method] => Operation was canceled."
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
                    [ExpenseManagment Module]-[ExpenseManagment_SecurityRepository class]-[DecryptExpenseData Method] => 
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

    public async Task<Util_GenericResponse<Expense>>
    DecryptExpenseData
    (
        string userId,
        Expense expense,
        Guid tag,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            byte[]? userKey = await groupKeySecurity.DeriveUserKey(iterations: 10000, outputLength: 32, userId, expense.GroupId, tag);

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
                "[ExpenseManagment Module]-[ExpenseManagment_SecurityRepository class]-[DecryptExpenseDataAsync Method] => Operation was canceled."
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
                    [ExpenseManagment Module]-[ExpenseManagment_SecurityRepository class]-[DecryptExpenseData Method] => 
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

    public static string
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