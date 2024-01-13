using SafeShare.DataAccessLayer.Models.SafeShareApiKey;
using SafeShare.DataTransormObject.SafeShareApiKey.ApiKey;

namespace SafeShare.Mappings.SafeShareApiKey;

public static class ApiKeyMappingExtension
{
    public static DTO_GetApiKey
    ToDto
    (
        this ApiKey apiKey
    )
    {
        if (apiKey == null) return null;

        return new DTO_GetApiKey
        {
            ApiKeyId = apiKey.ApiKeyId,
            ApiClientId = Guid.Parse(apiKey.ApiClientId),
            CreatedOn = apiKey.CreatedOn,
            Environment = apiKey.Environment,
            ExpiresOn = apiKey.ExpiresOn,
            IsActive = apiKey.IsActive,
            KeyHash = apiKey.KeyHash
        };
    }

    public static ApiKey
    ToEntity
    (
        this DTO_CreateApiKey createDto
    )
    {
        return new ApiKey
        {
            Environment = createDto.Environment,
            ApiClientId = createDto.UserId.ToString(),
            CreatedOn = DateTime.UtcNow,
            IsActive = createDto.Active,
            ExpiresOn = createDto.ExpiresOn
        };
    }

    public static void UpdateWith
    (
        this ApiKey apiKey,
        DTO_UpdateApiKey updateDto
    )
    {
        apiKey.IsActive = updateDto.IsActive;
        apiKey.Environment = updateDto.Environment;
    }
}