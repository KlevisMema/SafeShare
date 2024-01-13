using API_Client.BLL.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using SafeShare.DataAccessLayer.Context;
using SafeShare.Mappings.SafeShareApiKey;
using SafeShare.Utilities.SafeShareApiKey.Helpers;
using SafeShare.DataAccessLayer.Models.SafeShareApiKey;
using SafeShare.DataTransormObject.SafeShareApiKey.User;

namespace API_Client.BLL.Implementation;

public class ApiClientService
(
    ApiClientDbContext context, 
    ILogger<ApiClientService> logger, 
    UserManager<ApiClient> _userManager
) : IApiClientService
{
    public async Task<ServiceResponse<DTO_GetApiClient>> 
    CreateApiClient
    (
        DTO_CreateApiClient createDto
    )
    {
        logger.LogInformation("Creating a new API client");
        try
        {
            var apiClient = createDto.ToEntity();

            var result = await _userManager.CreateAsync(apiClient, createDto.Password);

            if (!result.Succeeded)
            {
                logger.LogWarning("Failed to create API client: {Errors}", result.Errors);
                return ServiceResponse<DTO_GetApiClient>.Fail("Failed to create API client.");
            }

            var dto = apiClient.ToDto();
            logger.LogInformation("API client created with ID {ApiClientId}", apiClient.Id);
            return new ServiceResponse<DTO_GetApiClient>(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating an API client");
            return ServiceResponse<DTO_GetApiClient>.Fail("Error occurred while creating the API client.");
        }
    }

    public async Task<ServiceResponse<DTO_GetApiClient>> 
    UpdateApiClient
    (
        Guid id, 
        DTO_UpdateApiClient updateDto
    )
    {
        logger.LogInformation("Updating API client {ApiClientId}", id);
        try
        {
            var apiClient = await context.Clients.FindAsync(id.ToString());
            if (apiClient == null)
            {
                logger.LogWarning("API client {ApiClientId} not found", id);
                return ServiceResponse<DTO_GetApiClient>.Fail("API client not found.");
            }

            apiClient.Email = updateDto.Email;
            apiClient.CompanyName = updateDto.CompanyName;
            apiClient.Description = updateDto.Description;
            apiClient.IsActive = updateDto.IsActive;
            apiClient.Website = updateDto.Website;
            apiClient.ContactPerson = updateDto.ContactPerson;
            apiClient.SiteYouDevelopingUrl = updateDto.SiteYouDevelopingUrl;
            apiClient.UserName = updateDto.UserName;

            var updateResult = await _userManager.UpdateAsync(apiClient);
            if (!updateResult.Succeeded)
            {
                logger.LogError("Failed to update API client {ApiClientId}. Errors: {Errors}", id, updateResult.Errors);
                return ServiceResponse<DTO_GetApiClient>.Fail("Failed to update API client.");
            }

            logger.LogInformation("API client {ApiClientId} updated successfully", id);
            return new ServiceResponse<DTO_GetApiClient>(apiClient.ToDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating API client {ApiClientId}", id);
            return ServiceResponse<DTO_GetApiClient>.Fail("Error occurred while updating the API client.");
        }
    }

    public async Task<ServiceResponse<DTO_GetApiClient>> 
    GetApiClient
    (
        Guid id
    )
    {
        logger.LogInformation("Retrieving API client {ApiClientId}", id);
        try
        {
            var apiClient = await context.Clients.FindAsync(id.ToString());
            if (apiClient == null)
            {
                logger.LogWarning("API client {ApiClientId} not found", id);
                return ServiceResponse<DTO_GetApiClient>.Fail("API client not found.");
            }

            logger.LogInformation("API client {ApiClientId} retrieved successfully", id);
            return new ServiceResponse<DTO_GetApiClient>(apiClient.ToDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving API client {ApiClientId}", id);
            return ServiceResponse<DTO_GetApiClient>.Fail("Error occurred while retrieving the API client.");
        }
    }

    public async Task<ServiceResponse<bool>> 
    DeleteApiClient
    (
        Guid id
    )
    {
        logger.LogInformation("Deleting API client {ApiClientId}", id);
        try
        {
            var apiClient = await context.Clients.FindAsync(id.ToString());
            if (apiClient == null)
            {
                logger.LogWarning("API client {ApiClientId} not found", id);
                return ServiceResponse<bool>.Fail("API client not found.");
            }

            context.Clients.Remove(apiClient);
            await context.SaveChangesAsync();

            logger.LogInformation("API client {ApiClientId} deleted successfully", id);
            return new ServiceResponse<bool>(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting API client {ApiClientId}", id);
            return ServiceResponse<bool>.Fail("Error occurred while deleting the API client.");
        }
    }
}