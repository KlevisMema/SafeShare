namespace SafeShare.ProxyApi.Helper;

public class API_Helper_ProxyForwardedHeadersMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        string clientIpAddress = context.Connection.RemoteIpAddress.ToString();
        context.Request.Headers.Add("X-Original-Client-IP", clientIpAddress);

        await next(context);
    }
}