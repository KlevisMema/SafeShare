using SafeShare.ClientServerShared.Routes;
using SafeShare.ClientServices.Interfaces;

namespace SafeShare.ClientServices.Authentication;

public class ClientAuthentication_TokenRefreshService(IHttpClientFactory httpClientFactory) : IClientAuthentication_TokenRefreshService
{
    private const string Client = "MyHttpClient";

    public async Task<bool>
    RefreshToken()
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var response = await httpClient.PostAsync(BaseRoute.RouteAuthenticationProxy + Route_AuthenticationRoute.RefreshToken, null);

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}