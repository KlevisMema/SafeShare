namespace SafeShare.API.Helpers;

public class API_Helper_CookieSettings
{
    public const string SectionName = "CookieSettings";
    public string AuthTokenCookieName { get; set; } = string.Empty;
    public string RefreshAuthTokenCookieName { get; set; } = string.Empty;
    public string RefreshAuthTokenIdCookieName { get; set; } = string.Empty;
}