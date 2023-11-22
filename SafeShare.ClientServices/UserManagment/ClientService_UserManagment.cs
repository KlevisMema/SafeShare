using System.Net.Http;
using System.Text.Json;
using SafeShare.ClientServerShared.Routes;
using SafeShare.ClientServices.Interfaces;
using SafeShare.ClientUtilities.Responses;
using SafeShare.ClientDTO.AccountManagment;

namespace SafeShare.ClientServices.UserManagment;

public class ClientService_UserManagment(HttpClient httpClient) : IClientService_UserManagment
{
    private static readonly JsonSerializerOptions s_writeOptions = new()
    {
        WriteIndented = true
    };

    public async Task<ClientUtil_ApiResponse<bool>> 
    CallTheApi()
    {
        try
        {

            var deactivateAccountData = new Dictionary<string, string>
            {
                { nameof(ClientDto_DeactivateAccount.Email), "memaklevis2@gmail.com" },
                { nameof(ClientDto_DeactivateAccount.Password), "Pa$$w0rd" }
            };

            var content = new FormUrlEncodedContent(deactivateAccountData);

            var response = await httpClient.PostAsync(BaseRoute.RouteAccountManagmentForClient + Route_AccountManagmentRoute.DeactivateAccount.Replace("{userId}", "f995febb-58b1-40b8-a2fb-a5ea6fe774e1"), content);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<bool>>(responseContent, s_writeOptions);

            return readResult!;
        }
        catch (Exception ex)
        {

        }

        return null!;

    }
}