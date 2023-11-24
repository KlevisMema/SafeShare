namespace SafeShare.API.Helpers;

internal class API_Helper_JwtCookieToHeaderMiddleware(RequestDelegate next)
{
    public async Task
    Invoke
    (
        HttpContext context
    )
    {
        if (context.Request.Cookies.TryGetValue("authToken", out string token))
            context.Request.Headers.Append("Authorization", "Bearer " + token);

        await next(context);
    }
}