using Azure.Core;
using SafeShare.Security.JwtSecurity.Helpers;

namespace SafeShare.ProxyApi.Helpers;

internal static class API_Helper_ExtractInfoFromRequestCookie
{
    public static string
    GetUserIp
    (
        HttpRequest Request
    )
    {
        return Request.Headers["X-Original-Client-IP"].ToString();
    }

    public static string
    UserId
    (
        string jwtToken
    )
    {
        return Helper_JwtToken.GetUserIdDirectlyFromJwtToken(jwtToken) ?? string.Empty;
    }

    public static string
    JwtToken
    (
        HttpRequest Request
    )
    {
        return Request.Cookies["AuthToken"] ?? string.Empty;
    }

    public static string
    GetForgeryToken
    (
        HttpRequest Request
    )
    {
        return Request.Cookies["XSRF-TOKEN"]?.ToString() ?? string.Empty;
    }

    public static string
    GetAspNetCoreForgeryToken
    (
        HttpRequest Request
    )
    {
        return Request.Cookies[".AspNetCore.Antiforgery.NcD0snFZIjg"]?.ToString() ?? string.Empty;
    }
}