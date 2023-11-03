using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SafeShare.Utilities.User;

public static class Util_FindUserIdFromToken
{
    public static string
    UserId
    (
        IHttpContextAccessor httpContextAccessor
    )
    {
        string? userId = httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return "";

        return userId;
    }
}