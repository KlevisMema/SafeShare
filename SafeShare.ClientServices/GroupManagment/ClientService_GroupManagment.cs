using System.Text;
using System.Net.Http;
using System.Text.Json;
using SafeShare.ClientDTO.GroupManagment;
using SafeShare.ClientServerShared.Routes;
using SafeShare.ClientUtilities.Responses;
using SafeShare.ClientServices.Interfaces;

namespace SafeShare.ClientServices.GroupManagment;
public class ClientService_GroupManagment(IHttpClientFactory httpClientFactory) : IClientService_GroupManagment
{
    private const string Client = "MyHttpClient";

    public async Task<ClientUtil_ApiResponse<ClientDto_GroupTypes>>
    GetGroupTypes()
    {
        var httpClient = httpClientFactory.CreateClient(Client);

        var response = await httpClient.GetAsync("api/GroupManagmentProxy/GroupTypes");

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<ClientDto_GroupTypes>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return readResult!;
    }
}