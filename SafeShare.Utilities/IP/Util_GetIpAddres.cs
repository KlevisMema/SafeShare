using Microsoft.AspNetCore.Http;

namespace SafeShare.Utilities.IP;
public static class Util_GetIpAddres
{
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
                    //else
                    //    // log
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