/*
 * Utility class dedicated to retrieving the IP address from a given HTTP context.
 * The utility can identify local IP addresses and attempt to fetch the external IP when necessary.
*/

using Microsoft.AspNetCore.Http;

namespace SafeShare.Utilities.IP;

/// <summary>
/// Provides utility methods related to IP address retrieval and location determination.
/// </summary>
public static class Util_GetIpAddres
{
    /// <summary>
    /// Retrieves the location based on the IP address from the given HTTP context accessor.
    /// If the IP address is local (":::1"), it tries to fetch the external IP using an external service.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <returns>A string representation of the IP address or an empty string in case of failure.</returns>
    public static async Task<string> GetLocation(IHttpContextAccessor httpContextAccessor)
    {
        try
        {
            string? result = httpContextAccessor.HttpContext.Connection?.RemoteIpAddress.ToString();

            if (result != null)
            {

                if (result == ":::1")
                {
                    HttpResponseMessage response;

                    var request = new HttpRequestMessage(HttpMethod.Get, "https://api.ipify.org/");
                    var client = new HttpClient();
                    response = client.Send(request);
                    if (response.IsSuccessStatusCode)
                    {
                        using var responseStream = response.Content.ReadAsStream();
                        StreamReader reader = new StreamReader(responseStream);
                        result = reader.ReadToEnd();
                    }
                }

                return result;
            }

        }
        catch (Exception ex)
        {
            // log
        }

        return "";
    }
}