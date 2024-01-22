using System.Text;
using System.Net.Http;
using System.Text.Json;
using SafeShare.ClientDTO.Authentication;
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
        HttpResponseMessage response = new();

        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            response = await httpClient.GetAsync(BaseRoute.RouteGroupManagmentProxy + Route_GroupManagmentRoutes.ProxyGroupTypes);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<ClientDto_GroupTypes>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception)
        {
            return new ClientUtil_ApiResponse<ClientDto_GroupTypes>()
            {
                Message = "Something went wrong, groups were not retrieved",
                Errors = null,
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = null
            };
        }
    }

    public async Task<ClientUtil_ApiResponse<ClientDto_GroupDetails>>
    GetGroupDetails
    (
        Guid groupId
    )
    {
        HttpResponseMessage response = new();

        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            response = await httpClient.GetAsync(BaseRoute.RouteGroupManagmentProxy + Route_GroupManagmentRoutes.ProxyGetGroupDetails.Replace("{groupId}", groupId.ToString()));

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<ClientDto_GroupDetails>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception)
        {
            return new ClientUtil_ApiResponse<ClientDto_GroupDetails>()
            {
                Message = "Something went wrong, group details were not retrieved",
                Errors = null,
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = null
            };
        }
    }

    public async Task<ClientUtil_ApiResponse<ClientDto_GroupType>>
    CreateGroup
    (
        ClientDto_CreateGroup createGroup
    )
    {
        HttpResponseMessage response = new();

        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var registerData = new Dictionary<string, string>
            {
                { nameof(ClientDto_CreateGroup.GroupName), createGroup.GroupName },
                { nameof(ClientDto_CreateGroup.GroupDescription), createGroup.GroupDescription },
            };

            var contentForm = new FormUrlEncodedContent(registerData);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteGroupManagmentProxy + Route_GroupManagmentRoutes.ProxyCreateGroup)
            {
                Content = contentForm
            };

            response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<ClientDto_GroupType>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception)
        {
            return new ClientUtil_ApiResponse<ClientDto_GroupType>()
            {
                Message = "Something went wrong, group was not created",
                Errors = null,
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = null
            };
        }
    }


    public async Task<ClientUtil_ApiResponse<ClientDto_GroupType>>
    EditGroup
    (
        Guid groupId,
        ClientDto_EditGroup editGroup
    )
    {
        HttpResponseMessage response = new();

        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var registerData = new Dictionary<string, string>
            {
                { nameof(ClientDto_EditGroup.GroupName), editGroup.GroupName },
                { nameof(ClientDto_EditGroup.GroupDescription), editGroup.GroupDescription },
            };

            var contentForm = new FormUrlEncodedContent(registerData);

            response = await httpClient.PutAsync(BaseRoute.RouteGroupManagmentProxy + Route_GroupManagmentRoutes.ProxyEditGroup.Replace("{groupId}", groupId.ToString()), contentForm);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<ClientDto_GroupType>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception)
        {
            return new ClientUtil_ApiResponse<ClientDto_GroupType>()
            {
                Message = "Something went wrong, group was not edited",
                Errors = null,
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = null
            };
        }
    }

    public async Task<ClientUtil_ApiResponse<bool>>
    DeleteGroup
    (
        Guid groupId
    )
    {
        HttpResponseMessage response = new();

        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            response = await httpClient.DeleteAsync(BaseRoute.RouteGroupManagmentProxy + Route_GroupManagmentRoutes.ProxyDeleteGroup.Replace("{groupId}", groupId.ToString()));

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception)
        {
            return new ClientUtil_ApiResponse<bool>()
            {
                Message = "Something went wrong, group was not deleted",
                Errors = null,
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = false
            };
        }
    }

    public async Task<ClientUtil_ApiResponse<List<ClientDto_RecivedInvitations>>>
    GetGroupsInvitations()
    {
        HttpResponseMessage response = new();

        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            response = await httpClient.GetAsync(BaseRoute.RouteGroupManagmentProxy + Route_GroupManagmentRoutes.ProxyGetGroupsInvitations);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<List<ClientDto_RecivedInvitations>>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception)
        {
            return new ClientUtil_ApiResponse<List<ClientDto_RecivedInvitations>>()
            {
                Message = "Something went wrong, group invitations were not retrieved",
                Errors = null,
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = null
            };
        }
    }

    public async Task<ClientUtil_ApiResponse<List<ClientDto_SentInvitations>>>
    GetSentGroupInvitations()
    {
        HttpResponseMessage response = new();

        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            response = await httpClient.GetAsync(BaseRoute.RouteGroupManagmentProxy + Route_GroupManagmentRoutes.ProxyGetSentGroupInvitations);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<List<ClientDto_SentInvitations>>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception)
        {
            return new ClientUtil_ApiResponse<List<ClientDto_SentInvitations>>()
            {
                Message = "Something went wrong, sent group invitations were not retrieved",
                Errors = null,
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = null
            };
        }
    }

    public async Task<ClientUtil_ApiResponse<bool>>
    SendInvitation
    (
        ClientDto_SendInvitationRequest sendInvitationRequest
    )
    {
        HttpResponseMessage response = new();

        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var json = JsonSerializer.Serialize(sendInvitationRequest);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            response = await httpClient.PostAsync(BaseRoute.RouteGroupManagmentProxy + Route_GroupManagmentRoutes.ProxySendInvitation, content);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception ex)
        {
            return new ClientUtil_ApiResponse<bool>()
            {
                Message = "Something went wrong, invitation was not sent",
                Errors = null,
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = false
            };
        }
    }

    public async Task<ClientUtil_ApiResponse<bool>>
    AcceptInvitation
    (
        ClientDto_InvitationRequestActions invitationRequestActions
    )
    {
        HttpResponseMessage response = new();

        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var json = JsonSerializer.Serialize(invitationRequestActions);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            response = await httpClient.PostAsync(BaseRoute.RouteGroupManagmentProxy + Route_GroupManagmentRoutes.ProxyAcceptInvitation, content);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception)
        {
            return new ClientUtil_ApiResponse<bool>()
            {
                Message = "Something went wrong, invitation was not accepted",
                Errors = null,
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = false
            };
        }
    }

    public async Task<ClientUtil_ApiResponse<bool>>
    RejectInvitation
    (
        ClientDto_InvitationRequestActions invitationRequestActions
    )
    {
        HttpResponseMessage response = new();

        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var json = JsonSerializer.Serialize(invitationRequestActions);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            response = await httpClient.PostAsync(BaseRoute.RouteGroupManagmentProxy + Route_GroupManagmentRoutes.ProxyRejectInvitation, content);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception)
        {
            return new ClientUtil_ApiResponse<bool>()
            {
                Message = "Something went wrong, invitation was not rejected",
                Errors = null,
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = false
            };
        }
    }

    public async Task<ClientUtil_ApiResponse<bool>>
    DeleteInvitation
    (
        ClientDto_InvitationRequestActions invitationRequestActions
    )
    {
        HttpResponseMessage response = new();

        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var content = new StringContent(JsonSerializer.Serialize(invitationRequestActions), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Delete, BaseRoute.RouteGroupManagmentProxy + Route_GroupManagmentRoutes.ProxyDeleteInvitation)
            {
                Content = content
            };

            response = await httpClient.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception)
        {
            return new ClientUtil_ApiResponse<bool>()
            {
                Message = "Something went wrong, invitation was not rejected",
                Errors = null,
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = false
            };
        }
    }

    public async Task<ClientUtil_ApiResponse<bool>>
    DeleteUsersFromGroup
    (
        Guid groupId,
        List<ClientDto_UsersGroupDetails> usersOfTheGroup
    )
    {
        HttpResponseMessage response = new();

        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var content = new StringContent(JsonSerializer.Serialize(usersOfTheGroup), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Delete, BaseRoute.RouteGroupManagmentProxy + Route_GroupManagmentRoutes.ProxyDeleteUsersFromGroup.Replace("{groupId}", groupId.ToString()))
            {
                Content = content
            };

            response = await httpClient.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception)
        {
            return new ClientUtil_ApiResponse<bool>()
            {
                Message = "Something went wrong, user was not removed form the group",
                Errors = null,
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = false
            };
        }
    }
}