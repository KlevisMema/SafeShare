using SafeShare.ClientServerShared.Routes;
using SafeShare.ClientServices.Interfaces;

namespace SafeShare.ClientServices.Authentication;

public class ClientAuthentication_TokenRefreshService(IHttpClientFactory httpClientFactory) : IClientAuthentication_TokenRefreshService
{
    public async Task<bool>
    RefreshToken()
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient("MyHttpClient");

            // Call the refresh token API endpoint
            var response = await httpClient.PostAsync(BaseRoute.RouteAuthenticationForClient + Route_AuthenticationRoute.RefreshToken, null);

            if (response.IsSuccessStatusCode)
            {
                // Token refreshed successfully
                return true;
            }
        }
        catch
        {
            // Handle exceptions
        }

        return false;
    }
}