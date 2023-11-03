/*
 * An authorization filter to enforce API key-based security. 
 * Ensures that requests to the application are authorized using the correct API key.
*/


using Microsoft.AspNetCore.Mvc;
using SafeShare.Security.Decryption;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SafeShare.Security.API;

/// <summary>
/// Represents an API key authorization filter for application authorization.
/// </summary>
public class ApiKeyAuthorizationFilter : IAuthorizationFilter, IApiKeyAuthorizationFilter
{
    /// <summary>
    /// The API key used for authorization.
    /// </summary>
    private readonly string _apiKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyAuthorizationFilter"/> class.
    /// </summary>
    /// <param name="apiKey">The API key used for authorization.</param>
    public ApiKeyAuthorizationFilter
    (
        string apiKey
    )
    {
        _apiKey = apiKey;
    }

    /// <summary>
    /// Called when the authorization is being performed.
    /// </summary>
    /// <param name="context">The authorization filter context.</param>
    public void
    OnAuthorization
    (
        AuthorizationFilterContext context
    )
    {
        string? encryptedApiKey = context.HttpContext.Request.Headers["X-Api-Key"].FirstOrDefault();

        if (string.IsNullOrEmpty(encryptedApiKey))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        string decryptedApiKey;
        try
        {
            decryptedApiKey = Security_DecryptHelper.DecryptWithPrivateKey(encryptedApiKey);
        }
        catch
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (decryptedApiKey != _apiKey)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}