using SafeShare.Utilities.IP;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SafeShare.Security.API.ActionFilters;

public class VerifyUser : ActionFilterAttribute
{
    private readonly ILogger<VerifyUser> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public VerifyUser
    (
        ILogger<VerifyUser> logger,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

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