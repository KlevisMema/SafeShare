/*
 * An authorization filter to enforce API key-based security. 
 * Ensures that requests to the application are authorized using the correct API key.
*/


using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using SafeShare.Security.API.Decryption;
using SafeShare.Security.API.Interfaces;
using SafeShare.Utilities.IP;

namespace SafeShare.Security.API.Imeplementations;

/// <summary>
/// Represents an API key authorization filter for application authorization.
/// </summary>
public class ApiKeyAuthorizationFilter : IAuthorizationFilter, IApiKeyAuthorizationFilter
{
    /// <summary>
    /// The API key used for authorization.
    /// </summary>
    private readonly string _apiKey = null!;
    /// <summary>
    /// The http accessor
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor = null!;
    /// <summary>
    /// The logger
    /// </summary>
    private readonly ILogger<ApiKeyAuthorizationFilter> _logger = null!;
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
    /// Initializes a new instance of the <see cref="ApiKeyAuthorizationFilter"/> class.
    /// </summary> 
    /// <param name="logger">The logged used for logging informations.</param>
    /// <param name="httpContextAccessor">
    ///     The http context accessor used to get the ip of the user making a request
    /// </param>
    public ApiKeyAuthorizationFilter
    (
        IHttpContextAccessor httpContextAccessor,
        ILogger<ApiKeyAuthorizationFilter> logger
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }
    /// <summary>
    /// Called when the authorization is being performed.
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

            _logger.Log
            (
                LogLevel.Error,
                """
                    [Security Module]-[ApiKeyAuthorizationFilter class]-[OnAuthorization Method]
                    IP {IP} tried to make a request to api and the encrypted
                    API key is null or empty so the user is its unauthorized.
                    Result {@Result}
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
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

                _logger.Log
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
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    context.Result
                );

                return;
            }
        }
        catch (Exception ex)
        {
            context.Result = new UnauthorizedResult();

            _logger.Log
            (
                LogLevel.Critical,
                """
                    [Security Module]-[ApiKeyAuthorizationFilter class]-[OnAuthorization Method]
                    IP {IP} tried to make a request to api and an exception happened.
                    Exception : {@ex}, Result {@Result}
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                ex,
                context.Result
            );

            return;
        }

        if (decryptedApiKey != _apiKey)
        {
            context.Result = new UnauthorizedResult();

            _logger.Log
            (
                LogLevel.Error,
                """
                    [Security Module]-[ApiKeyAuthorizationFilter class]-[OnAuthorization Method]
                    IP {IP} tried to make a request to api and the decrypted key is not equal 
                    to the key that is required to access this api.
                    Result {@Result}
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                context.Result
            );
        }
    }
}