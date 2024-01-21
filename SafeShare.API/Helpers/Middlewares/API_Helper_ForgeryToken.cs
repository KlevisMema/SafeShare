using Microsoft.AspNetCore.Antiforgery;

namespace SafeShare.API.Helpers.Middlewares;

/// <summary>
/// A middleware that assigns a token to the client response.
/// </summary>
public class API_Helper_ForgeryToken(IAntiforgery antiforgery, RequestDelegate next)
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (String.IsNullOrEmpty(context.Request.Cookies["XSRF-TOKEN"]) &&
            context.User.Identity.IsAuthenticated)
        {
            var token = antiforgery.GetAndStoreTokens(context);

            context.Response.Cookies.Append("XSRF-TOKEN", token.RequestToken, new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Strict,
            });
        }

        await next(context);
    }
}