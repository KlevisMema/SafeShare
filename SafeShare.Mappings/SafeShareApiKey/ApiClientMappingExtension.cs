using SafeShare.DataAccessLayer.Models.SafeShareApiKey;
using SafeShare.DataTransormObject.SafeShareApiKey.User;

namespace SafeShare.Mappings.SafeShareApiKey;

public static class ApiClientMappingExtension
{
    public static ApiClient
    ToEntity
    (
        this DTO_CreateApiClient createDto
    )
    {
        return new ApiClient
        {
            CompanyName = createDto.CompanyName,
            Description = createDto.Description,
            Website = createDto.Website,
            ContactPerson = createDto.ContactPerson,
            SiteYouDevelopingUrl = createDto.SiteYouDevelopingUrl,
            RegisteredOn = DateTime.UtcNow,
            IsActive = false,
            Email = createDto.Email,
            PhoneNumber = createDto.PhoneNumber,
            NormalizedEmail = createDto.Email.ToUpper(),
            NormalizedUserName = createDto.UserName,
            UserName = createDto.UserName,
        };
    }

    public static DTO_GetApiClient
    ToDto
    (
        this ApiClient client
    )
    {
        return new DTO_GetApiClient
        {
            CompanyName = client.CompanyName,
            Description = client.Description,
            RegisteredOn = client.RegisteredOn,
            IsActive = client.IsActive,
            Website = client.Website,
            ContactPerson = client.ContactPerson,
            SiteYouDevelopingUrl = client.SiteYouDevelopingUrl,
            Email = client.Email
        };
    }
}