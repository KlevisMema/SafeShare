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
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var response = await httpClient.GetAsync(BaseRoute.RouteGroupManagmentProxy + Route_GroupManagmentRoutes.ProxyGroupTypes);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<ClientDto_GroupTypes>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return readResult!;
        }
        catch (Exception)
        {
            return new ClientUtil_ApiResponse<ClientDto_GroupTypes>
            {
                Succsess = false
            };
        }
    }

    public async Task<ClientUtil_ApiResponse<ClientDto_GroupDetails>>
    GetGroupDetails
    (
        Guid groupId
    )
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var response = await httpClient.GetAsync(BaseRoute.RouteGroupManagmentProxy + Route_GroupManagmentRoutes.ProxyGetGroupDetails.Replace("{groupId}", groupId.ToString()));

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<ClientDto_GroupDetails>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return readResult!;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<ClientUtil_ApiResponse<ClientDto_GroupType>>
    CreateGroup
    (
        ClientDto_CreateGroup createGroup
    )
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var registerData = new Dictionary<string, string>
            {
                { nameof(ClientDto_CreateGroup.GroupName), createGroup.GroupName },
                { nameof(ClientDto_CreateGroup.GroupDescription), createGroup.GroupDescription },
            };

            var contentForm = new FormUrlEncodedContent(registerData);

            var response = await httpClient.PostAsync(BaseRoute.RouteGroupManagmentProxy + Route_GroupManagmentRoutes.ProxyCreateGroup, contentForm);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<ClientDto_GroupType>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return readResult!;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<ClientUtil_ApiResponse<ClientDto_GroupType>>
    EditGroup
    (
        Guid groupId,
        ClientDto_EditGroup editGroup
    )
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var registerData = new Dictionary<string, string>
            {
                { nameof(ClientDto_EditGroup.GroupName), editGroup.GroupName },
                { nameof(ClientDto_EditGroup.GroupDescription), editGroup.GroupDescription },
            };

            var contentForm = new FormUrlEncodedContent(registerData);

            var response = await httpClient.PutAsync(BaseRoute.RouteGroupManagmentProxy + Route_GroupManagmentRoutes.ProxyEditGroup.Replace("{groupId}", groupId.ToString()), contentForm);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<ClientDto_GroupType>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return readResult!;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<ClientUtil_ApiResponse<ClientDto_GroupType>>
    DeleteGroup
    (
        Guid groupId
    )
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var response = await httpClient.DeleteAsync(BaseRoute.RouteGroupManagmentProxy + Route_GroupManagmentRoutes.ProxyEditGroup.Replace("{groupId}", groupId.ToString()));

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<ClientDto_GroupType>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return readResult!;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<ClientUtil_ApiResponse<List<ClientDto_RecivedInvitations>>>
    GetGroupsInvitations()
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var response = await httpClient.GetAsync(BaseRoute.RouteGroupManagmentProxy + Route_GroupManagmentRoutes.ProxyGetGroupsInvitations);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<List<ClientDto_RecivedInvitations>>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return readResult!;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<ClientUtil_ApiResponse<List<ClientDto_SentInvitations>>>
    GetSentGroupInvitations()
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var response = await httpClient.GetAsync(BaseRoute.RouteGroupManagmentProxy + Route_GroupManagmentRoutes.ProxyGetSentGroupInvitations);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<List<ClientDto_SentInvitations>>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return readResult!;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<ClientUtil_ApiResponse<bool>>
    SendInvitation
    (
        ClientDto_SendInvitationRequest sendInvitationRequest
    )
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var json = JsonSerializer.Serialize(sendInvitationRequest);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(BaseRoute.RouteGroupManagmentProxy + Route_GroupManagmentRoutes.ProxySendInvitation, content);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return readResult!;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<ClientUtil_ApiResponse<bool>>
    AcceptInvitation
    (
        ClientDto_InvitationRequestActions invitationRequestActions
    )
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var json = JsonSerializer.Serialize(invitationRequestActions);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(BaseRoute.RouteGroupManagmentProxy + Route_GroupManagmentRoutes.ProxyAcceptInvitation, content);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return readResult!;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<ClientUtil_ApiResponse<bool>>
    RejectInvitation
    (
        ClientDto_InvitationRequestActions invitationRequestActions
    )
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var json = JsonSerializer.Serialize(invitationRequestActions);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(BaseRoute.RouteGroupManagmentProxy + Route_GroupManagmentRoutes.ProxyRejectInvitation, content);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return readResult!;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<ClientUtil_ApiResponse<bool>>
    DeleteInvitation
    (
        ClientDto_InvitationRequestActions invitationRequestActions
    )
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var content = new StringContent(JsonSerializer.Serialize(new { invitationRequestActions }), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Delete, BaseRoute.RouteGroupManagmentProxy + Route_GroupManagmentRoutes.ProxyDeleteInvitation)
            {
                Content = content
            };

            var response = await httpClient.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return readResult!;
        }
        catch (Exception)
        {

            throw;
        }
    }
}