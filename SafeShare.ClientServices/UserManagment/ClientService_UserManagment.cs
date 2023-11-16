using System.Net.Http;
using SafeShare.ClientServerShared.Routes;
using SafeShare.ClientDTO.AccountManagment;
using SafeShare.ClientServices.Interfaces;
using SafeShare.ClientUtilities.Responses;
using System.Text.Json;

namespace SafeShare.ClientServices.UserManagment;

public class ClientService_UserManagment : IClientService_UserManagment
{
    private readonly HttpClient _httpClient;

    public ClientService_UserManagment(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ClientUtil_ApiResponse<bool>> CallTheApi()
    {
        try
        {

            var deactivateAccountData = new Dictionary<string, string>
            {
                { nameof(ClientDto_DeactivateAccount.Email), "memaklevis2@gmail.com" },
                { nameof(ClientDto_DeactivateAccount.Password), "Pa$$w0rd" }
            };

            var content = new FormUrlEncodedContent(deactivateAccountData);

            var response = await _httpClient.PostAsync(BaseRoute.RouteAccountManagmentForClient + Route_AccountManagmentRoute.DeactivateAccount.Replace("{userId}", "f995febb-58b1-40b8-a2fb-a5ea6fe774e1"), content);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = System.Text.Json.JsonSerializer.Deserialize<ClientUtil_ApiResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return readResult!;
        }
        catch (Exception ex)
        {

        }

        return null!;

    }
}