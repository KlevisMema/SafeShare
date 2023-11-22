using System.Text.Json;
using SafeShare.ClientDTO.Authentication;
using SafeShare.ClientServerShared.Routes;
using SafeShare.ClientUtilities.Responses;
using SafeShare.ClientDTO.AccountManagment;
using SafeShare.ClientServices.Interfaces;

namespace SafeShare.ClientServices.Authentication;

public class AuthenticationService(HttpClient httpClient) : IAuthenticationService
{
    private static readonly JsonSerializerOptions s_writeOptions = new()
    {
        WriteIndented = false
    };

    public async Task<ClientUtil_ApiResponse<ClientDto_LoginResult>>
    LogInUser
    (
        ClientDto_Login login
    )
    {
        try
        {
            var loginData = new Dictionary<string, string>
            {
                { nameof(ClientDto_Login.Email), login.Email },
                { nameof(ClientDto_Login.Password), login.Password }
            };

            var contentForm = new FormUrlEncodedContent(loginData);

            var response = await httpClient.PostAsync(BaseRoute.RouteAuthenticationForClient + Route_AuthenticationRoute.Login, contentForm);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<ClientDto_LoginResult>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return readResult!;
        }
        catch (Exception ex)
        {

            throw;
        }
    }
}