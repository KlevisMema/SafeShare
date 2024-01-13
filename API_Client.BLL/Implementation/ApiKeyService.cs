using System.Text;
using Microsoft.AspNetCore.Mvc;
using API_Client.BLL.Interfaces;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SafeShare.DataAccessLayer.Context;
using SafeShare.Mappings.SafeShareApiKey;
using Microsoft.AspNetCore.Http.HttpResults;
using SafeShare.Utilities.SafeShareApiKey.Helpers;
using SafeShare.DataTransormObject.SafeShareApiKey.ApiKey;

namespace API_Client.BLL.Implementation;

public class ApiKeyService(ApiClientDbContext context, ILogger<ApiKeyService> logger) : IApiKeyService
{
    public async Task<ServiceResponse<IEnumerable<DTO_GetApiKey>>>
    GetApiKeys
    (
        Guid clientId
    )
    {
        logger.LogInformation("Retrieving API keys for client {ClientId}", clientId);
        try
        {
            var apiKeys = await context.ApiKeys
                .Where(x => x.ApiClientId == clientId.ToString())
                .Select(x => x.ToDto())
                .ToListAsync();

            return new ServiceResponse<IEnumerable<DTO_GetApiKey>>(apiKeys);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving API keys for client {ClientId}", clientId);
            return ServiceResponse<IEnumerable<DTO_GetApiKey>>.Fail("Error retrieving API keys.");
        }
    }

    public async Task<ServiceResponse<DTO_GetApiKey>>
    GetApiKey
    (
        Guid id,
        Guid clientId
    )
    {
        logger.LogInformation("Retrieving API key {KeyId} for client {ClientId}", id, clientId);
        try
        {
            var apiKey = await context.ApiKeys
                .Where(x => x.ApiKeyId == id && x.ApiClientId == clientId.ToString())
                .Select(x => x.ToDto())
                .FirstOrDefaultAsync();

            if (apiKey == null)
            {
                logger.LogWarning("API key {KeyId} not found for client {ClientId}", id, clientId);
                return ServiceResponse<DTO_GetApiKey>.Fail("API key not found.");
            }

            return new ServiceResponse<DTO_GetApiKey>(apiKey);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving API key {KeyId} for client {ClientId}", id, clientId);
            return ServiceResponse<DTO_GetApiKey>.Fail("Error retrieving API key.");
        }
    }

    public async Task<ServiceResponse<DTO_GetApiKey>>
    PostApiKey
    (
        DTO_CreateApiKey createDto
    )
    {
        logger.LogInformation("Creating a new API key for client {ClientId}", createDto.UserId);
        try
        {
            var user = await context.Users.FindAsync(createDto.UserId.ToString());

            if (user is null || !user.IsActive)
            {
                logger.LogInformation("API key was not created invalid client {ClientId}", createDto.UserId);
                return ServiceResponse<DTO_GetApiKey>.Fail("User doesn't exists");
            }

            var apiKey = createDto.ToEntity();

            var gererateApiKey = HashHelper.GenerateApiKey(createDto.UserId.ToString());
            var hashedKey = HashHelper.HashApiKey(gererateApiKey);
            apiKey.KeyHash = hashedKey;

            context.ApiKeys.Add(apiKey);
            await context.SaveChangesAsync();

            logger.LogInformation("API key created successfully with ID {KeyId} for client {ClientId}", apiKey.ApiKeyId, createDto.UserId);
            return new ServiceResponse<DTO_GetApiKey>(new DTO_GetApiKey
            {
                IsActive = createDto.Active,
                ApiClientId = Guid.Parse(user.Id),
                ApiKeyId = apiKey.ApiKeyId,
                CreatedOn = apiKey.CreatedOn,
                Environment = createDto.Environment,
                ExpiresOn = apiKey.ExpiresOn,
                KeyHash = gererateApiKey
            }, "Please store the key somewhere you will not be able to retrieve the value again.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating API key for client {ClientId}", createDto.UserId);
            return ServiceResponse<DTO_GetApiKey>.Fail("Error occurred while creating the API key.");
        }
    }

    public async Task<ServiceResponse<bool>>
    PutApiKey
    (
        Guid id,
        Guid clientId,
        DTO_UpdateApiKey updateDto
    )
    {
        logger.LogInformation("Updating API key {KeyId} for client {ClientId}", id, clientId);
        try
        {
            var apiKey = await context.ApiKeys.FindAsync(id);
            if (apiKey == null || apiKey.ApiClientId != clientId.ToString())
            {
                logger.LogWarning("API key {KeyId} not found or client ID mismatch for client {ClientId}", id, clientId);
                return ServiceResponse<bool>.Fail("API key not found or client ID mismatch.");
            }

            apiKey.UpdateWith(updateDto);
            context.Entry(apiKey).State = EntityState.Modified;
            await context.SaveChangesAsync();

            logger.LogInformation("API key {KeyId} updated successfully for client {ClientId}", id, clientId);
            return new ServiceResponse<bool>(true, "Update succsessfull");
        }
        catch (DbUpdateConcurrencyException ex)
        {
            logger.LogError(ex, "Concurrency error occurred while updating API key {KeyId} for client {ClientId}", id, clientId);
            return ServiceResponse<bool>.Fail("Concurrency error occurred while updating the API key.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred while updating API key {KeyId} for client {ClientId}", id, clientId);
            return ServiceResponse<bool>.Fail("An unexpected error occurred while updating the API key.");
        }
    }

    public async Task<ServiceResponse<bool>>
    DeleteApiKey
    (
        Guid id,
        Guid clientId
    )
    {
        logger.LogInformation("Attempting to delete API key {KeyId} for client {ClientId}", id, clientId);
        try
        {
            var apiKey = await context.ApiKeys.FindAsync(id);
            if (apiKey == null || apiKey.ApiClientId != clientId.ToString())
            {
                logger.LogWarning("API key {KeyId} not found or client ID mismatch for client {ClientId}", id, clientId);
                return ServiceResponse<bool>.Fail("API key not found or client ID mismatch.");
            }

            context.ApiKeys.Remove(apiKey);
            await context.SaveChangesAsync();

            logger.LogInformation("API key {KeyId} successfully deleted for client {ClientId}", id, clientId);
            return new ServiceResponse<bool>(true, "Api key deleted succsessfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting API key {KeyId} for client {ClientId}", id, clientId);
            return ServiceResponse<bool>.Fail("Error occurred while deleting the API key.");
        }
    }
}