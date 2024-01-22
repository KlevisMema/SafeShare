using System.Net;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using SafeShare.ProxyApi.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SafeShare.ClientServerShared.Routes;
using SafeShare.Security.JwtSecurity.Helpers;
using SafeShare.ProxyApi.Container.Interfaces;
using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.DataTransormObject.SafeShareApi.Security;
using SafeShare.DataTransormObject.SafeShareApi.UserManagment;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment;
using SafeShare.DataTransormObject.SafeShareApi.ExpenseManagment;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment.GroupInvitations;

namespace SafeShare.ProxyApi.Container.Services;

public class GroupManagmentProxyService
(
    IHttpClientFactory httpClientFactory,
    ILogger<GroupManagmentProxyService> logger,
    IHubContext<NotificationHubProxyService> _hubContext,
    IOptions<API_Helper_RequestHeaderSettings> requestHeaderOptions,
    IRequestConfigurationProxyService requestConfigurationProxyService
) : IGroupManagmentProxyService
{
    public async Task<Tuple<Util_GenericResponse<DTO_GroupsTypes>, HttpResponseMessage>>
    GetGroupTypes
    (
        string userId,
        string userIp,
        string jwtToken
    )
    {
        try
        {
            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userId), userId),
                (nameof(userIp), userIp),
                (nameof(jwtToken), jwtToken)
            );

            var httpClient = API_Helper_HttpClient.CreateClientInstance(requestConfigurationProxyService.GetClient(), httpClientFactory);

            var content = new StringContent(JsonSerializer.Serialize(new { userId }), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, BaseRoute.RouteGroupManagmentForClient + Route_GroupManagmentRoutes.GroupTypes.Replace("{userId}", userId.ToString()))
            {
                Content = content
            };

            API_Helper_HttpClient.AddHeadersToTheRequest
            (
                jwtToken,
                requestMessage,
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ClientIP, userIp),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ApiKey, requestConfigurationProxyService.GetApiKey())
            );

            var response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<DTO_GroupsTypes>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return Tuple.Create(readResult, response);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in GetGroupTypes.");

            return Tuple.Create
            (
                new Util_GenericResponse<DTO_GroupsTypes>()
                {
                    Message = "Something went wrong",
                    Errors = null,
                    StatusCode = HttpStatusCode.InternalServerError,
                    Succsess = false,
                    Value = null
                }, new HttpResponseMessage()
            );
        }
    }

    public async Task<Util_GenericResponse<DTO_GroupDetails>>
    GetGroupDetails
    (
        string userId,
        string userIp,
        string jwtToken,
        Guid groupId
    )
    {
        try
        {
            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userId), userId),
                (nameof(jwtToken), jwtToken)
            );

            var httpClient = API_Helper_HttpClient.CreateClientInstance(requestConfigurationProxyService.GetClient(), httpClientFactory);

            var content = new StringContent(JsonSerializer.Serialize(new { userId }), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, BaseRoute.RouteGroupManagmentForClient + Route_GroupManagmentRoutes.GetGroupDetails.Replace("{userId}", userId.ToString()).Replace("{groupId}", groupId.ToString()))
            {
                Content = content
            };

            API_Helper_HttpClient.AddHeadersToTheRequest
            (
                jwtToken,
                requestMessage,
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ClientIP, userIp),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ApiKey, requestConfigurationProxyService.GetApiKey())
            );

            var response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<DTO_GroupDetails>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in GetGroupTypes.");

            return new Util_GenericResponse<DTO_GroupDetails>
            {
                Errors = null,
                Message = "Something went wrong",
                StatusCode = HttpStatusCode.InternalServerError,
                Succsess = false,
                Value = null
            };
        }
    }

    public async Task<Util_GenericResponse<DTO_GroupType>>
    CreateGroup
    (
        string userId,
        string userIp,
        string jwtToken,
        string fogeryToken,
        string aspNetForgeryToken,
        DTO_CreateGroup createGroup
    )
    {
        try
        {
            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userIp), userIp),
                (nameof(userId), userId),
                (nameof(jwtToken), jwtToken),
                (nameof(fogeryToken), fogeryToken),
                (nameof(aspNetForgeryToken), aspNetForgeryToken)
            );

            var httpClient = API_Helper_HttpClient.NewClientWithCookies
            (
                requestConfigurationProxyService.GetBaseAddrOfMainApi(),
                aspNetForgeryToken,
                fogeryToken,
                requestHeaderOptions.Value.AspNetCoreAntiforgery,
                requestHeaderOptions.Value.XSRF_TOKEN
            );

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

            API_Helper_HttpClient.AddHeadersToTheRequest
            (
                jwtToken,
                requestMessage,
                new KeyValuePair<string, string>(requestHeaderOptions.Value.XSRF_TOKEN, fogeryToken),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.AspNetCoreAntiforgery, aspNetForgeryToken),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ClientIP, userIp),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ApiKey, requestConfigurationProxyService.GetApiKey())
            );

            var response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<DTO_GroupType>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in CreateGroup.");

            return new Util_GenericResponse<DTO_GroupType>
            {
                Errors = null,
                Message = "Something went wrong",
                StatusCode = HttpStatusCode.InternalServerError,
                Succsess = false,
                Value = null
            };
        }
    }

    public async Task<Util_GenericResponse<DTO_GroupType>>
    EditGroup
    (
        string userId,
        string userIp,
        string fogeryToken,
        string aspNetForgeryToken,
        string jwtToken,
        Guid groupId,
        DTO_EditGroup editGroup
    )
    {
        try
        {
            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userIp), userIp),
                (nameof(userId), userId),
                (nameof(jwtToken), jwtToken),
                (nameof(fogeryToken), fogeryToken),
                (nameof(aspNetForgeryToken), aspNetForgeryToken)
            );

            var httpClient = API_Helper_HttpClient.NewClientWithCookies
            (
                requestConfigurationProxyService.GetBaseAddrOfMainApi(),
                aspNetForgeryToken,
                fogeryToken,
                requestHeaderOptions.Value.AspNetCoreAntiforgery,
                requestHeaderOptions.Value.XSRF_TOKEN
            );

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

            API_Helper_HttpClient.AddHeadersToTheRequest
            (
                jwtToken,
                requestMessage,
                new KeyValuePair<string, string>(requestHeaderOptions.Value.XSRF_TOKEN, fogeryToken),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.AspNetCoreAntiforgery, aspNetForgeryToken),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ClientIP, userIp),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ApiKey, requestConfigurationProxyService.GetApiKey())
            );

            var response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<DTO_GroupType>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in EditGroup.");

            return new Util_GenericResponse<DTO_GroupType>
            {
                Errors = null,
                Message = "Something went wrong",
                StatusCode = HttpStatusCode.InternalServerError,
                Succsess = false,
                Value = null
            };
        }
    }

    public async Task<Util_GenericResponse<bool>>
    DeleteGroup
    (
        string userId,
        string userIp,
        string fogeryToken,
        string aspNetForgeryToken,
        string jwtToken,
        Guid groupId
    )
    {
        try
        {

            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userIp), userIp),
                (nameof(userId), userId),
                (nameof(jwtToken), jwtToken),
                (nameof(fogeryToken), fogeryToken),
                (nameof(aspNetForgeryToken), aspNetForgeryToken)
            );

            var httpClient = API_Helper_HttpClient.NewClientWithCookies
            (
                requestConfigurationProxyService.GetBaseAddrOfMainApi(),
                aspNetForgeryToken,
                fogeryToken,
                requestHeaderOptions.Value.AspNetCoreAntiforgery,
                requestHeaderOptions.Value.XSRF_TOKEN
            );

            var content = new StringContent(JsonSerializer.Serialize(new { userId, groupId }), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, BaseRoute.RouteGroupManagmentForClient + Route_GroupManagmentRoutes.DeleteGroup.Replace("{userId}", userId.ToString()).Replace("{groupId}", groupId.ToString()))
            {
                Content = content
            };

            API_Helper_HttpClient.AddHeadersToTheRequest
            (
                jwtToken,
                requestMessage,
                new KeyValuePair<string, string>(requestHeaderOptions.Value.XSRF_TOKEN, fogeryToken),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.AspNetCoreAntiforgery, aspNetForgeryToken),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ClientIP, userIp),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ApiKey, requestConfigurationProxyService.GetApiKey())
            );

            var response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in DeleteGroup.");

            return new Util_GenericResponse<bool>
            {
                Errors = null,
                Message = "Something went wrong",
                StatusCode = HttpStatusCode.InternalServerError,
                Succsess = false,
                Value = false
            };
        }
    }

    public async Task<Util_GenericResponse<List<DTO_RecivedInvitations>>>
    GetGroupsInvitations
    (
        string userId,
        string userIp,
        string jwtToken
    )
    {
        try
        {
            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userIp), userIp),
                (nameof(userId), userId),
                (nameof(jwtToken), jwtToken)
            );

            var httpClient = API_Helper_HttpClient.CreateClientInstance(requestConfigurationProxyService.GetClient(), httpClientFactory);

            var content = new StringContent(JsonSerializer.Serialize(new { userId }), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, BaseRoute.RouteGroupManagmentForClient + Route_GroupManagmentRoutes.GetGroupsInvitations.Replace("{userId}", userId.ToString()))
            {
                Content = content
            };

            API_Helper_HttpClient.AddHeadersToTheRequest
            (
                jwtToken,
                requestMessage,
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ClientIP, userIp),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ApiKey, requestConfigurationProxyService.GetApiKey())
            );

            var response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<List<DTO_RecivedInvitations>>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in GetGroupsInvitations.");

            return new Util_GenericResponse<List<DTO_RecivedInvitations>>
            {
                Errors = null,
                Message = "Something went wrong",
                StatusCode = HttpStatusCode.InternalServerError,
                Succsess = false,
                Value = null
            };
        }
    }

    public async Task<Util_GenericResponse<List<DTO_SentInvitations>>>
    GetSentGroupInvitations
    (
        string userId,
        string userIp,
        string jwtToken
    )
    {
        try
        {
            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userIp), userIp),
                (nameof(userId), userId),
                (nameof(jwtToken), jwtToken)
            );

            var httpClient = API_Helper_HttpClient.CreateClientInstance(requestConfigurationProxyService.GetClient(), httpClientFactory);

            var content = new StringContent(JsonSerializer.Serialize(new { userId }), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, BaseRoute.RouteGroupManagmentForClient + Route_GroupManagmentRoutes.GetSentGroupInvitations.Replace("{userId}", userId.ToString()))
            {
                Content = content
            };

            API_Helper_HttpClient.AddHeadersToTheRequest
            (
                jwtToken,
                requestMessage,
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ClientIP, userIp),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ApiKey, requestConfigurationProxyService.GetApiKey())
            );

            var response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<List<DTO_SentInvitations>>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in GetSentGroupInvitations.");

            return new Util_GenericResponse<List<DTO_SentInvitations>>
            {
                Errors = null,
                Message = "Something went wrong",
                StatusCode = HttpStatusCode.InternalServerError,
                Succsess = false,
                Value = null
            };
        }
    }

    public async Task<Util_GenericResponse<bool>>
    SendInvitation
    (
        string userId,
        string userIp,
        string jwtToken,
        string fogeryToken,
        string aspNetForgeryToken,
        DTO_SendInvitationRequest dTO_SendInvitation
    )
    {
        try
        {
            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userIp), userIp),
                (nameof(userId), userId),
                (nameof(jwtToken), jwtToken),
                (nameof(fogeryToken), fogeryToken),
                (nameof(aspNetForgeryToken), aspNetForgeryToken)
            );

            var httpClient = API_Helper_HttpClient.NewClientWithCookies
            (
                requestConfigurationProxyService.GetBaseAddrOfMainApi(),
                aspNetForgeryToken,
                fogeryToken,
                requestHeaderOptions.Value.AspNetCoreAntiforgery,
                requestHeaderOptions.Value.XSRF_TOKEN
            );

            dTO_SendInvitation.InvitingUserId = Guid.Parse(userId);

            var content = new StringContent(JsonSerializer.Serialize(dTO_SendInvitation), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteGroupManagmentForClient + Route_GroupManagmentRoutes.SendInvitation.Replace("{userId}", userId.ToString()))
            {
                Content = content
            };

            API_Helper_HttpClient.AddHeadersToTheRequest
            (
                jwtToken,
                requestMessage,
                new KeyValuePair<string, string>(requestHeaderOptions.Value.XSRF_TOKEN, fogeryToken),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.AspNetCoreAntiforgery, aspNetForgeryToken),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ClientIP, userIp),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ApiKey, requestConfigurationProxyService.GetApiKey())
            );

            var response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            if (readResult.Succsess)
                await _hubContext.Clients.User(dTO_SendInvitation.InvitedUserId.ToString()).SendAsync("ReceiveGroupInvitation");

            return readResult;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in SendInvitation.");

            return new Util_GenericResponse<bool>
            {
                Errors = null,
                Message = "Something went wrong",
                StatusCode = HttpStatusCode.InternalServerError,
                Succsess = false,
                Value = false
            };
        }
    }

    public async Task<Util_GenericResponse<bool>>
    AcceptInvitation
    (
        string userId,
        string userIp,
        string jwtToken,
        string fogeryToken,
        string aspNetForgeryToken,
        DTO_InvitationRequestActions acceptInvitationRequest
    )
    {
        try
        {
            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userIp), userIp),
                (nameof(userId), userId),
                (nameof(jwtToken), jwtToken),
                (nameof(fogeryToken), fogeryToken),
                (nameof(aspNetForgeryToken), aspNetForgeryToken)
            );

            var httpClient = API_Helper_HttpClient.NewClientWithCookies
            (
                requestConfigurationProxyService.GetBaseAddrOfMainApi(),
                aspNetForgeryToken,
                fogeryToken,
                requestHeaderOptions.Value.AspNetCoreAntiforgery,
                requestHeaderOptions.Value.XSRF_TOKEN
            );

            acceptInvitationRequest.InvitedUserId = Guid.Parse(userId);

            var content = new StringContent(JsonSerializer.Serialize(acceptInvitationRequest), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteGroupManagmentForClient + Route_GroupManagmentRoutes.AcceptInvitation.Replace("{userId}", userId.ToString()))
            {
                Content = content
            };

            API_Helper_HttpClient.AddHeadersToTheRequest
            (
                jwtToken,
                requestMessage,
                new KeyValuePair<string, string>(requestHeaderOptions.Value.XSRF_TOKEN, fogeryToken),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.AspNetCoreAntiforgery, aspNetForgeryToken),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ClientIP, userIp),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ApiKey, requestConfigurationProxyService.GetApiKey())
            );

            var response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            if (readResult.Succsess)
            {
                await _hubContext.Clients.User(acceptInvitationRequest.InvitingUserId.ToString())
                                         .SendAsync("AcceptedInvitation", acceptInvitationRequest.GroupName, acceptInvitationRequest.UserWhoAcceptedTheInvitation);
            }

            return readResult;

        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in AcceptInvitation.");

            return new Util_GenericResponse<bool>
            {
                Errors = null,
                Message = "Something went wrong",
                StatusCode = HttpStatusCode.InternalServerError,
                Succsess = false,
                Value = false
            };
        }
    }

    public async Task<Util_GenericResponse<bool>>
    RejectInvitation
    (
        string userId,
        string userIp,
        string jwtToken,
        string fogeryToken,
        string aspNetForgeryToken,
        DTO_InvitationRequestActions rejectInvitationRequest
    )
    {
        try
        {
            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userIp), userIp),
                (nameof(userId), userId),
                (nameof(jwtToken), jwtToken),
                (nameof(fogeryToken), fogeryToken),
                (nameof(aspNetForgeryToken), aspNetForgeryToken)
            );

            var httpClient = API_Helper_HttpClient.NewClientWithCookies
            (
                requestConfigurationProxyService.GetBaseAddrOfMainApi(),
                aspNetForgeryToken,
                fogeryToken,
                requestHeaderOptions.Value.AspNetCoreAntiforgery,
                requestHeaderOptions.Value.XSRF_TOKEN
            );

            rejectInvitationRequest.InvitedUserId = Guid.Parse(userId);

            var content = new StringContent(JsonSerializer.Serialize(rejectInvitationRequest), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteGroupManagmentForClient + Route_GroupManagmentRoutes.RejectInvitation.Replace("{userId}", userId.ToString()))
            {
                Content = content
            };

            API_Helper_HttpClient.AddHeadersToTheRequest
            (
                jwtToken,
                requestMessage,
                new KeyValuePair<string, string>(requestHeaderOptions.Value.XSRF_TOKEN, fogeryToken),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.AspNetCoreAntiforgery, aspNetForgeryToken),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ClientIP, userIp),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ApiKey, requestConfigurationProxyService.GetApiKey())
            );

            var response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in RejectInvitation.");

            return new Util_GenericResponse<bool>
            {
                Errors = null,
                Message = "Something went wrong",
                StatusCode = HttpStatusCode.InternalServerError,
                Succsess = false,
                Value = false
            };
        }
    }

    public async Task<Util_GenericResponse<bool>>
    DeleteInvitation
    (
        string userId,
        string userIp,
        string jwtToken,
        string fogeryToken,
        string aspNetForgeryToken,
        DTO_InvitationRequestActions deleteInvitationRequest
    )
    {
        try
        {
            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userIp), userIp),
                (nameof(userId), userId),
                (nameof(jwtToken), jwtToken),
                (nameof(fogeryToken), fogeryToken),
                (nameof(aspNetForgeryToken), aspNetForgeryToken)
            );

            var httpClient = API_Helper_HttpClient.NewClientWithCookies
            (
                requestConfigurationProxyService.GetBaseAddrOfMainApi(),
                aspNetForgeryToken,
                fogeryToken,
                requestHeaderOptions.Value.AspNetCoreAntiforgery,
                requestHeaderOptions.Value.XSRF_TOKEN
            );

            deleteInvitationRequest.InvitingUserId = Guid.Parse(userId);

            var content = new StringContent(JsonSerializer.Serialize(deleteInvitationRequest), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, BaseRoute.RouteGroupManagmentForClient + Route_GroupManagmentRoutes.DeleteInvitation.Replace("{userId}", userId.ToString()))
            {
                Content = content
            };

            API_Helper_HttpClient.AddHeadersToTheRequest
            (
                jwtToken,
                requestMessage,
                new KeyValuePair<string, string>(requestHeaderOptions.Value.XSRF_TOKEN, fogeryToken),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.AspNetCoreAntiforgery, aspNetForgeryToken),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ClientIP, userIp),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ApiKey, requestConfigurationProxyService.GetApiKey())
            );

            var response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in DeleteInvitation.");

            return new Util_GenericResponse<bool>
            {
                Errors = null,
                Message = "Something went wrong",
                StatusCode = HttpStatusCode.InternalServerError,
                Succsess = false,
                Value = false
            };
        }
    }

    public async Task<Util_GenericResponse<bool>>
    DeleteUsersFromGroup
    (
        string userId,
        string userIp,
        string jwtToken,
        string fogeryToken,
        string aspNetForgeryToken,
        Guid groupId,
        List<DTO_UsersGroupDetails> UsersToRemoveFromGroup
    )
    {
        try
        {
            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userIp), userIp),
                (nameof(userId), userId),
                (nameof(jwtToken), jwtToken),
                (nameof(fogeryToken), fogeryToken),
                (nameof(aspNetForgeryToken), aspNetForgeryToken)
            );

            var httpClient = API_Helper_HttpClient.NewClientWithCookies
            (
                requestConfigurationProxyService.GetBaseAddrOfMainApi(),
                aspNetForgeryToken,
                fogeryToken,
                requestHeaderOptions.Value.AspNetCoreAntiforgery,
                requestHeaderOptions.Value.XSRF_TOKEN
            );

            var content = new StringContent(JsonSerializer.Serialize(UsersToRemoveFromGroup), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, BaseRoute.RouteGroupManagmentForClient + Route_GroupManagmentRoutes.DeleteUsersFromGroup.Replace("{userId}", userId.ToString()).Replace("{groupId}", groupId.ToString()))
            {
                Content = content
            };

            API_Helper_HttpClient.AddHeadersToTheRequest
            (
                jwtToken,
                requestMessage,
                new KeyValuePair<string, string>(requestHeaderOptions.Value.XSRF_TOKEN, fogeryToken),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.AspNetCoreAntiforgery, aspNetForgeryToken),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ClientIP, userIp),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ApiKey, requestConfigurationProxyService.GetApiKey())
            );

            var response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            if (readResult.Succsess)
                await _hubContext.Clients
                                 .Users(UsersToRemoveFromGroup.Select(x => x.UserId))
                                 .SendAsync("RemovedFromTheGroup", UsersToRemoveFromGroup.Select(x => x.GroupName).FirstOrDefault(), groupId);

            return readResult;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in DeleteUsersFromGroup.");

            return new Util_GenericResponse<bool>
            {
                Errors = null,
                Message = "Something went wrong",
                StatusCode = HttpStatusCode.InternalServerError,
                Succsess = false,
                Value = false
            };
        }
    }
}