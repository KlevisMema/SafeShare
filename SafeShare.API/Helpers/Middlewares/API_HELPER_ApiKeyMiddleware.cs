/* 
 * Contains helper classes and middleware for handling various aspects of the SafeShare API.
 */

using Microsoft.EntityFrameworkCore;
using SafeShare.DataAccessLayer.Context;
using SafeShare.Utilities.SafeShareApiKey.Helpers;

namespace SafeShare.API.Helpers.Middlewares;

/// <summary>
/// Middleware for validating API keys in incoming HTTP requests.
/// </summary>
///  /// <remarks>
/// Initializes a new instance of the <see cref="API_HELPER_ApiKeyMiddleware"/> class.
/// </remarks>
/// <param name="next">The next middleware delegate in the pipeline.</param>
/// <param name="logger">Logger for logging information and errors.</param>
/// <param name="_scopeFactory">The factory for creating service scopes.</param>
public class API_HELPER_ApiKeyMiddleware(RequestDelegate next, ILogger<API_HELPER_ApiKeyMiddleware> logger, IServiceScopeFactory _scopeFactory)
{
    /// <summary>
    /// Invokes the middleware to check the API key in the request.
    /// </summary>
    /// <param name="context">The HttpContext for the current request.</param>
    public async Task
    InvokeAsync
    (
        HttpContext context
    )
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApiClientDbContext>();

            var apiKeyHeader = context.Request.Headers["X-Api-Key"].FirstOrDefault();

            if (string.IsNullOrEmpty(apiKeyHeader))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key is missing");
                return;
            }

            var hashApiKey = HashHelper.HashApiKey(apiKeyHeader);
            if (hashApiKey == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }

            var exists = await dbContext.ApiKeys.AnyAsync(x => x.KeyHash == hashApiKey);
            if (!exists)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }

            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "An error happened in API_HELPER_ApiKeyMiddleware");
        }
    }
}