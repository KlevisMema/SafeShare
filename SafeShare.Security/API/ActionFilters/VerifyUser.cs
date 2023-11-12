/* 
 * Provides an action filter for verifying if the user making a request is authorized based on the user ID.
 * This filter compares the user ID provided in the route with the user ID obtained from the JWT token in the HTTP context.
 */

using SafeShare.Utilities.IP;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SafeShare.Security.API.ActionFilters;

/// <summary>
/// Action filter for verifying the identity of the user making a request.
/// Compares the user ID in the route against the user ID from the JWT token.
/// </summary>
public class VerifyUser : ActionFilterAttribute
{
    /// <summary>
    /// Logger for logging warnings and errors.
    /// </summary>
    private readonly ILogger<VerifyUser> _logger;
    /// <summary>
    /// Accessor to obtain the current HTTP context.
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor;
    /// <summary>
    /// Initializes a new instance of the <see cref="VerifyUser"/> class.
    /// </summary>
    /// <param name="logger">Logger for logging warnings and errors.</param>
    /// <param name="httpContextAccessor">Accessor to obtain the current HTTP context.</param>
    public VerifyUser
    (
        ILogger<VerifyUser> logger,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }
    /// <summary>
    /// Executes before the action method is invoked.
    /// Verifies if the user ID from the token matches the user ID in the route.
    /// If they don't match, the request is unauthorized.
    /// </summary>
    /// <param name="context">Context for the action being executed.</param>
    public override void
    OnActionExecuting
    (
        ActionExecutingContext context
    )
    {
        base.OnActionExecuting(context);

        var ownerIdParam = context.ActionArguments["userId"] as Guid?;
        var userIdFromToken = Util_FindUserIdFromToken.UserId(_httpContextAccessor);

        if (ownerIdParam.HasValue && ownerIdParam.Value.ToString() != userIdFromToken)
        {
            _logger.Log
            (
                LogLevel.Warning,
                """
                    A request with this [IP] {IP} was made and the user 
                    that should make the request did not made it.
                    The id from the route its not the same as the id in the 
                    jwt token.
                 """,
                Util_GetIpAddres.GetLocation(_httpContextAccessor)
            );

            context.Result = new UnauthorizedResult();
        }
    }
}