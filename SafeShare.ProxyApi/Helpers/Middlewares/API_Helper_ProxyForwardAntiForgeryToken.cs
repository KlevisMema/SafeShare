namespace SafeShare.ProxyApi.Helpers.Middlewares;

internal class API_Helper_ProxyForwardAntiForgeryToken(RequestDelegate next)
{
    public async Task
    Invoke
    (
        HttpContext context
    )
    {
        if (context.Request.Cookies.TryGetValue(".AspNetCore.Antiforgery.NcD0snFZIjg", out var forgeryTokenIdentifier) &&
            context.Request.Cookies.TryGetValue("XSRF-TOKEN", out var forgeryToken))
        {
            if (context.Request.Method == "POST" || context.Request.Method == "PUT" || context.Request.Method == "DELETE")
            {
                context.Request.Headers.Append("XSRF-TOKEN", forgeryToken);
                context.Request.Headers.Append(".AspNetCore.Antiforgery.NcD0snFZIjg", forgeryTokenIdentifier);
            }
        }

        await next(context);
    }
}