/*
 * An authorization filter to enforce API key-based security. 
 * Ensures that requests to the application are authorized using the correct API key.
*/


using SafeShare.Utilities.IP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using SafeShare.Security.API.Decryption;
using SafeShare.Security.API.Interfaces;

namespace SafeShare.Security.API.Imeplementations;

/// <summary>
/// Represents an API key authorization filter for enforcing application security.
/// This filter validates incoming requests using a specified API key.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ApiKeyAuthorizationFilter"/> class.
/// </remarks>
/// <param name="apiKey">The API key used for authorization.</param>
/// <param name="httpContextAccessor">The HTTP context accessor.</param>
/// <param name="logger">The logger used for logging information.</param>
public class ApiKeyAuthorizationFilter
(
    string apiKey,
    IHttpContextAccessor httpContextAccessor,
    ILogger<ApiKeyAuthorizationFilter> logger
) : IAuthorizationFilter, IApiKeyAuthorizationFilter
{
    /// <summary>
    /// Called when authorization is being performed on a request.
    /// Checks if the request includes a valid API key for access authorization.
    /// </summary>
    /// <param name="context">The authorization filter context.</param>
    public async void
    OnAuthorization
    (
        AuthorizationFilterContext context
    )
    {
        string? encryptedApiKey = context.HttpContext.Request.Headers["X-Api-Key"].FirstOrDefault();

        if (string.IsNullOrEmpty(encryptedApiKey))
        {
            context.Result = new UnauthorizedResult();

            logger.Log
            (
                LogLevel.Error,
                """
                    [Security Module]-[ApiKeyAuthorizationFilter class]-[OnAuthorization Method]
                    IP {IP} tried to make a request to api and the encrypted
                    API key is null or empty so the user is its unauthorized.
                    Result {@Result}
                 """,
                await Util_GetIpAddres.GetLocation(httpContextAccessor),
                context.Result
            );

            return;
        }

        string? decryptedApiKey;

        try
        {
            decryptedApiKey = Security_DecryptHelper.DecryptWithPrivateKey(encryptedApiKey);

            if (decryptedApiKey == null)
            {
                context.Result = new UnauthorizedResult();

                logger.Log
                (
                    LogLevel.Error,
                    """
                        [Security Module]-[ApiKeyAuthorizationFilter class]-[OnAuthorization Method]
                        IP {IP} tried to make a request to api and the decrypted
                        API key is null so the user is its unauthorized.
                        Result {@Result}.
                        Check if the enviroment variable is present, that might be a reason why 
                        the decryption is failing due to a null privated key.
                     """,
                    await Util_GetIpAddres.GetLocation(httpContextAccessor),
                    context.Result
                );

                return;
            }
        }
        catch (Exception ex)
        {
            context.Result = new UnauthorizedResult();

            logger.Log
            (
                LogLevel.Critical,
                """
                    [Security Module]-[ApiKeyAuthorizationFilter class]-[OnAuthorization Method]
                    IP {IP} tried to make a request to api and an exception happened.
                    Exception : {@ex}, Result {@Result}
                 """,
                await Util_GetIpAddres.GetLocation(httpContextAccessor),
                ex,
                context.Result
            );

            return;
        }

        if (decryptedApiKey != apiKey)
        {
            context.Result = new UnauthorizedResult();

            logger.Log
            (
                LogLevel.Error,
                """
                    [Security Module]-[ApiKeyAuthorizationFilter class]-[OnAuthorization Method]
                    IP {IP} tried to make a request to api and the decrypted key is not equal 
                    to the key that is required to access this api.
                    Result {@Result}
                 """,
                await Util_GetIpAddres.GetLocation(httpContextAccessor),
                context.Result
            );
        }
    }
}