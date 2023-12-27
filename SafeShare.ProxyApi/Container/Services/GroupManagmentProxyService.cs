using System.Text;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using SafeShare.ClientServerShared.Routes;
using SafeShare.Security.JwtSecurity.Helpers;
using SafeShare.ProxyApi.Container.Interfaces;
using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.DataTransormObject.SafeShareApi.Security;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.SafeShareApi.ExpenseManagment;
using SendGrid.Helpers.Mail;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment.GroupInvitations;

namespace SafeShare.ProxyApi.Container.Services;

public class GroupManagmentProxyService(IHttpClientFactory httpClientFactory) : IGroupManagmentProxyService
{
    private const string Client = "ProxyHttpClient";
    private readonly string ApiKey = Environment.GetEnvironmentVariable("SAFE_SHARE_API_KEY") ?? string.Empty;

    public async Task<Util_GenericResponse<DTO_GroupsTypes>>
    GetGroupTypes
    (
        string jwtToken
    )
    {
        var userId = Helper_JwtToken.GetUserIdDirectlyFromJwtToken(jwtToken);

        var httpClient = httpClientFactory.CreateClient(Client);

        var content = new StringContent(JsonSerializer.Serialize(new { userId }), Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, BaseRoute.RouteGroupManagmentForClient + Route_GroupManagmentRoutes.GroupTypes.Replace("{userId}", userId.ToString()))
        {
            Content = content
        };
        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<DTO_GroupsTypes>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return readResult ?? new Util_GenericResponse<DTO_GroupsTypes>();
    }

    public async Task<Util_GenericResponse<DTO_GroupDetails>>
    GetGroupDetails
    (
        string userId,
        string jwtToken,
        Guid groupId
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

        var content = new StringContent(JsonSerializer.Serialize(new { userId }), Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, BaseRoute.RouteGroupManagmentForClient + Route_GroupManagmentRoutes.GetGroupDetails.Replace("{userId}", userId.ToString()).Replace("{groupId}", groupId.ToString()))
        {
            Content = content
        };
        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<DTO_GroupDetails>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return readResult ?? new Util_GenericResponse<DTO_GroupDetails>();
    }

    public async Task<Util_GenericResponse<bool>>
    CreateGroup
    (
         string userId,
        string jwtToken,
        DTO_CreateGroup createGroup
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

        var expense = new Dictionary<string, string>
        {
            { nameof(DTO_CreateGroup.GroupName), createGroup.GroupName },
            { nameof(DTO_CreateGroup.GroupDescription), createGroup.GroupDescription},
        };

        var contentForm = new FormUrlEncodedContent(expense);

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteGroupManagmentForClient + Route_GroupManagmentRoutes.CreateGroup.Replace("{userId}", userId.ToString()))
        {
            Content = contentForm
        };
        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return readResult ?? new Util_GenericResponse<bool>();
    }

    public async Task<Util_GenericResponse<DTO_GroupType>>
    EditGroup
    (
        string userId,
        string jwtToken,
        Guid groupId,
        DTO_EditGroup editGroup
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

        var expense = new Dictionary<string, string>
        {
            { nameof(DTO_CreateGroup.GroupName), editGroup.GroupName },
            { nameof(DTO_CreateGroup.GroupDescription), editGroup.GroupDescription},
        };

        var contentForm = new FormUrlEncodedContent(expense);

        var requestMessage = new HttpRequestMessage(HttpMethod.Put, BaseRoute.RouteGroupManagmentForClient + Route_GroupManagmentRoutes.EditGroup.Replace("{userId}", userId.ToString()).Replace("{groupId}", groupId.ToString()))
        {
            Content = contentForm
        };
        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<DTO_GroupType>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return readResult ?? new Util_GenericResponse<DTO_GroupType>();
    }

    public async Task<Util_GenericResponse<bool>>
    DeleteGroup
    (
        string userId,
        string jwtToken,
        Guid groupId
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

        var content = new StringContent(JsonSerializer.Serialize(new { userId, groupId }), Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Delete, BaseRoute.RouteGroupManagmentForClient + Route_GroupManagmentRoutes.DeleteGroup.Replace("{userId}", userId.ToString()).Replace("{groupId}", groupId.ToString()))
        {
            Content = content
        };
        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return readResult ?? new Util_GenericResponse<bool>();
    }

    public async Task<Util_GenericResponse<List<DTO_RecivedInvitations>>>
    GetGroupsInvitations
    (
        string userId,
        string jwtToken
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

        var content = new StringContent(JsonSerializer.Serialize(new { userId }), Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, BaseRoute.RouteGroupManagmentForClient + Route_GroupManagmentRoutes.GetGroupsInvitations.Replace("{userId}", userId.ToString()))
        {
            Content = content
        };
        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<List<DTO_RecivedInvitations>>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return readResult ?? new Util_GenericResponse<List<DTO_RecivedInvitations>>();
    }

    public async Task<Util_GenericResponse<List<DTO_SentInvitations>>>
    GetSentGroupInvitations
    (
        string userId,
        string jwtToken
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

        var content = new StringContent(JsonSerializer.Serialize(new { userId }), Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, BaseRoute.RouteGroupManagmentForClient + Route_GroupManagmentRoutes.GetSentGroupInvitations.Replace("{userId}", userId.ToString()))
        {
            Content = content
        };
        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<List<DTO_SentInvitations>>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return readResult ?? new Util_GenericResponse<List<DTO_SentInvitations>>();
    }

    public async Task<Util_GenericResponse<bool>>
    SendInvitation
    (
        string userId,
        string jwtToken,
        DTO_SendInvitationRequest dTO_SendInvitation
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

        var content = new StringContent(JsonSerializer.Serialize(new { dTO_SendInvitation }), Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteGroupManagmentForClient + Route_GroupManagmentRoutes.SendInvitation.Replace("{userId}", userId.ToString()))
        {
            Content = content
        };
        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return readResult ?? new Util_GenericResponse<bool>();
    }

    public async Task<Util_GenericResponse<bool>>
    AcceptInvitation
    (
        string userId,
        string jwtToken,
        DTO_InvitationRequestActions acceptInvitationRequest
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

        var content = new StringContent(JsonSerializer.Serialize(new { acceptInvitationRequest }), Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteGroupManagmentForClient + Route_GroupManagmentRoutes.AcceptInvitation.Replace("{userId}", userId.ToString()))
        {
            Content = content
        };
        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return readResult ?? new Util_GenericResponse<bool>();
    }

    public async Task<Util_GenericResponse<bool>>
    RejectInvitation
    (
        string userId,
        string jwtToken,
        DTO_InvitationRequestActions rejectInvitationRequest
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

        var content = new StringContent(JsonSerializer.Serialize(new { rejectInvitationRequest }), Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteGroupManagmentForClient + Route_GroupManagmentRoutes.RejectInvitation.Replace("{userId}", userId.ToString()))
        {
            Content = content
        };
        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return readResult ?? new Util_GenericResponse<bool>();
    }

    public async Task<Util_GenericResponse<bool>>
    DeleteInvitation
    (
        string userId,
        string jwtToken,
        DTO_InvitationRequestActions deleteInvitationRequest
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

        var content = new StringContent(JsonSerializer.Serialize(new { deleteInvitationRequest }), Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteGroupManagmentForClient + Route_GroupManagmentRoutes.DeleteInvitation.Replace("{userId}", userId.ToString()))
        {
            Content = content
        };
        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return readResult ?? new Util_GenericResponse<bool>();
    }
}