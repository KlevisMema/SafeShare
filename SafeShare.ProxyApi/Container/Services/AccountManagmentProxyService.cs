using System.Net;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using SafeShare.ProxyApi.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.SignalR;
using SafeShare.ClientServerShared.Routes;
using SafeShare.ProxyApi.Container.Interfaces;
using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.DataTransormObject.SafeShareApi.UserManagment;
using SafeShare.DataTransormObject.SafeShareApi.Authentication;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment.GroupInvitations;

namespace SafeShare.ProxyApi.Container.Services;

public class AccountManagmentProxyService
(
    IHttpClientFactory httpClientFactory,
    ILogger<AccountManagmentProxyService> logger,
     IHubContext<NotificationHubProxyService> _hubContext,
    IOptions<API_Helper_RequestHeaderSettings> requestHeaderOptions,
    IRequestConfigurationProxyService requestConfigurationProxyService
) : IAccountManagmentProxyService
{
    public async Task<Util_GenericResponse<DTO_UserUpdatedInfo>>
    GetUser
    (
        string userId,
        string userIp,
        string jwtToken
    )
    {
        HttpResponseMessage response = new();

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

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, BaseRoute.RouteAccountManagmentForClient + Route_AccountManagmentRoute.GetUser.Replace("{userId}", userId.ToString()))
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

            response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<DTO_UserUpdatedInfo>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in GetUser.");

            return new Util_GenericResponse<DTO_UserUpdatedInfo>
            {
                Errors = null,
                Message = "Something went wrong",
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = null
            };
        }
    }

    public async Task<Tuple<Util_GenericResponse<DTO_UserUpdatedInfo>, HttpResponseMessage>>
    UpdateUser
    (
        string userId,
        string userIp,
        string jwtToken,
        string fogeryToken,
        string aspNetForgeryToken,
        DTO_UserInfo userInfo
    )
    {
        HttpResponseMessage response = new();

        try
        {
            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userId), userId),
                (nameof(userIp), userIp),
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

            var registerData = new Dictionary<string, string>
        {
            { nameof(DTO_UserInfo.FullName), userInfo.FullName },
            { nameof(DTO_UserInfo.UserName), userInfo.UserName },
            { nameof(DTO_UserInfo.Gender), userInfo.Gender.ToString() },
            { nameof(DTO_UserInfo.Birthday), userInfo.Birthday.ToString() },
            { nameof(DTO_UserInfo.PhoneNumber), userInfo.PhoneNumber },
            { nameof(DTO_UserInfo.Enable2FA), userInfo.Enable2FA.ToString() },
        };

            var contentForm = new FormUrlEncodedContent(registerData);

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, BaseRoute.RouteAccountManagmentForClient + Route_AccountManagmentRoute.UpdateUser.Replace("{userId}", userId))
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

            response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<DTO_UserUpdatedInfo>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })
                ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return Tuple.Create(readResult, response);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in UpdateUser.");

            return Tuple.Create(new Util_GenericResponse<DTO_UserUpdatedInfo>
            {
                Errors = null,
                Message = "Something went wrong",
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = null
            }, new HttpResponseMessage());
        }
    }

    public async Task<Util_GenericResponse<bool>>
    ChangePassword
    (
        string userId,
        string userIp,
        string jwtToken,
        string fogeryToken,
        string aspNetForgeryToken,
        DTO_UserChangePassword changePassword
    )
    {
        HttpResponseMessage response = new();

        try
        {
            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userId), userId),
                (nameof(userIp), userIp),
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

            var registerData = new Dictionary<string, string>
            {
                { nameof(DTO_UserChangePassword.OldPassword), changePassword.OldPassword },
                { nameof(DTO_UserChangePassword.NewPassword), changePassword.NewPassword },
                { nameof(DTO_UserChangePassword.ConfirmNewPassword), changePassword.ConfirmNewPassword},
            };

            var contentForm = new FormUrlEncodedContent(registerData);

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, BaseRoute.RouteAccountManagmentForClient + Route_AccountManagmentRoute.ChangePassword.Replace("{userId}", userId))
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

            response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in ChangePassword.");

            return new Util_GenericResponse<bool>
            {
                Errors = null,
                Message = "Something went wrong",
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = false
            };
        }
    }

    public async Task<Tuple<Util_GenericResponse<bool>, HttpResponseMessage>>
    DeactivateAccount
    (
        string userId,
        string userIp,
        string jwtToken,
        string fogeryToken,
        string aspNetForgeryToken,
        DTO_DeactivateAccount deactivateAccount
    )
    {
        HttpResponseMessage response = new();

        try
        {
            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userId), userId),
                (nameof(userIp), userIp),
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

            var registerData = new Dictionary<string, string>
            {
                { nameof(DTO_DeactivateAccount.Email), deactivateAccount.Email },
                { nameof(DTO_DeactivateAccount.Password), deactivateAccount.Password },
            };

            var contentForm = new FormUrlEncodedContent(registerData);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteAccountManagmentForClient + Route_AccountManagmentRoute.DeactivateAccount.Replace("{userId}", userId))
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

            response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return Tuple.Create(readResult, response);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in DeactivateAccount.");

            return Tuple.Create(new Util_GenericResponse<bool>
            {
                Errors = null,
                Message = "Something went wrong",
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = false
            }, new HttpResponseMessage());
        }
    }

    public async Task<Util_GenericResponse<bool>>
    ActivateAccountRequest
    (
        string email,
        string userIp
    )
    {
        HttpResponseMessage response = new();

        try
        {
            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userIp), userIp)
            );

            var httpClient = API_Helper_HttpClient.CreateClientInstance(requestConfigurationProxyService.GetClient(), httpClientFactory);

            var content = new StringContent(JsonSerializer.Serialize(new { email }), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteAccountManagmentForClient + Route_AccountManagmentRoute.ActivateAccountRequest + $"?email={email}")
            {
                Content = content
            };

            API_Helper_HttpClient.AddHeadersToTheRequest
            (
                string.Empty,
                requestMessage,
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ClientIP, userIp),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ApiKey, requestConfigurationProxyService.GetApiKey())
            );

            response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in ActivateAccountRequest.");

            return new Util_GenericResponse<bool>
            {
                Errors = null,
                Message = "Something went wrong",
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = false
            };
        }
    }

    public async Task<Util_GenericResponse<bool>>
    ActivateAccountRequestConfirmation
    (
        string userIp,
        DTO_ActivateAccountConfirmation activateAccountConfirmationDto
    )
    {
        HttpResponseMessage response = new();

        try
        {
            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userIp), userIp)
            );

            var httpClient = API_Helper_HttpClient.CreateClientInstance(requestConfigurationProxyService.GetClient(), httpClientFactory);

            var json = JsonSerializer.Serialize(activateAccountConfirmationDto);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteAccountManagmentForClient + Route_AccountManagmentRoute.ActivateAccountRequestConfirmation)
            {
                Content = content
            };

            API_Helper_HttpClient.AddHeadersToTheRequest
            (
                string.Empty,
                requestMessage,
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ClientIP, userIp),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ApiKey, requestConfigurationProxyService.GetApiKey())
            );

            response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in ActivateAccountRequestConfirmation.");

            return new Util_GenericResponse<bool>
            {
                Errors = null,
                Message = "Something went wrong",
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = false
            };
        }
    }

    public async Task<Util_GenericResponse<bool>>
    ForgotPassword
    (
        string userIp,
        DTO_ForgotPassword forgotPassword
    )
    {
        HttpResponseMessage response = new();

        try
        {
            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userIp), userIp)
            );

            var httpClient = API_Helper_HttpClient.CreateClientInstance(requestConfigurationProxyService.GetClient(), httpClientFactory);

            var registerData = new Dictionary<string, string>
            {
                { nameof(DTO_ForgotPassword.Email), forgotPassword.Email }
            };

            var contentForm = new FormUrlEncodedContent(registerData);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteAccountManagmentForClient + Route_AccountManagmentRoute.ForgotPassword)
            {
                Content = contentForm
            };

            API_Helper_HttpClient.AddHeadersToTheRequest
            (
                string.Empty,
                requestMessage,
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ClientIP, userIp),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ApiKey, requestConfigurationProxyService.GetApiKey())
            );

            response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in ForgotPassword.");

            return new Util_GenericResponse<bool>
            {
                Errors = null,
                Message = "Something went wrong",
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = false
            };
        }
    }

    public async Task<Util_GenericResponse<bool>>
    ResetPassword
    (
        string userIp,
        DTO_ResetPassword resetPassword
    )
    {
        HttpResponseMessage response = new();

        try
        {
            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userIp), userIp)
            );

            var httpClient = API_Helper_HttpClient.CreateClientInstance(requestConfigurationProxyService.GetClient(), httpClientFactory);

            var registerData = new Dictionary<string, string>
            {
                { nameof(DTO_ResetPassword.Email), resetPassword.Email },
                { nameof(DTO_ResetPassword.NewPassword), resetPassword.NewPassword },
                { nameof(DTO_ResetPassword.Token), resetPassword.Token },
                { nameof(DTO_ResetPassword.ConfirmNewPassword), resetPassword.ConfirmNewPassword },
            };

            var contentForm = new FormUrlEncodedContent(registerData);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteAccountManagmentForClient + Route_AccountManagmentRoute.ResetPassword)
            {
                Content = contentForm
            };

            API_Helper_HttpClient.AddHeadersToTheRequest
            (
                string.Empty,
                requestMessage,
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ClientIP, userIp),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ApiKey, requestConfigurationProxyService.GetApiKey())
            );

            response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in ResetPassword.");

            return new Util_GenericResponse<bool>
            {
                Errors = null,
                Message = "Something went wrong",
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = false
            };
        }
    }

    public async Task<Util_GenericResponse<bool>>
    RequestChangeEmail
    (
        string userId,
        string userIp,
        string jwtToken,
        string fogeryToken,
        string aspNetForgeryToken,
        DTO_ChangeEmailAddressRequest emailAddress
    )
    {
        HttpResponseMessage response = new();

        try
        {
            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userId), userId),
                (nameof(userIp), userIp),
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

            var registerData = new Dictionary<string, string>
            {
                { nameof(DTO_ChangeEmailAddressRequest.ConfirmNewEmailAddress), emailAddress.ConfirmNewEmailAddress },
                { nameof(DTO_ChangeEmailAddressRequest.NewEmailAddress), emailAddress.NewEmailAddress },
                { nameof(DTO_ChangeEmailAddressRequest.CurrentEmailAddress), emailAddress.CurrentEmailAddress }
            };

            var contentForm = new FormUrlEncodedContent(registerData);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteAccountManagmentForClient + Route_AccountManagmentRoute.RequestChangeEmail.Replace("{userId}", userId))
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

            response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in RequestChangeEmail.");

            return new Util_GenericResponse<bool>
            {
                Errors = null,
                Message = "Something went wrong",
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = false
            };
        }
    }

    public async Task<Tuple<Util_GenericResponse<bool>, HttpResponseMessage>>
    ConfirmChangeEmailAddressRequest
    (
        string userId,
        string userIp,
        string jwtToken,
        string fogeryToken,
        string aspNetForgeryToken,
        DTO_ChangeEmailAddressRequestConfirm changeEmailAddressConfirmDto
    )
    {
        HttpResponseMessage response = new();

        try
        {
            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userId), userId),
                (nameof(userIp), userIp),
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

            var json = JsonSerializer.Serialize(changeEmailAddressConfirmDto);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteAccountManagmentForClient + Route_AccountManagmentRoute.ConfirmChangeEmailRequest.Replace("{userId}", userId))
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

            response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            if (readResult.Succsess)
                await _hubContext.Clients.User(userId).SendAsync("EmailChanged", changeEmailAddressConfirmDto.EmailAddress);

            return Tuple.Create(readResult, response);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in ConfirmChangeEmailAddressRequest.");

            return Tuple.Create(new Util_GenericResponse<bool>
            {
                Errors = null,
                Message = "Something went wrong",
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = false
            }, new HttpResponseMessage());
        }
    }

    public async Task<Util_GenericResponse<List<DTO_UserSearched>>>
    SearchUserByUserName
    (
        string userId,
        string userIp,
        string jwtToken,
        string userName,
        CancellationToken cancellationToken
    )
    {
        HttpResponseMessage response = new();

        try
        {
            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userId), userId),
                (nameof(userIp), userIp),
                (nameof(jwtToken), jwtToken)
            );

            var httpClient = API_Helper_HttpClient.CreateClientInstance(requestConfigurationProxyService.GetClient(), httpClientFactory);

            var content = new StringContent(JsonSerializer.Serialize(new { userName }), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, BaseRoute.RouteAccountManagmentForClient + Route_AccountManagmentRoute.SearchUserByUserName.Replace("{userId}", userId.ToString()) + $"?username={userName}")
            {
                Content = content,

            };

            API_Helper_HttpClient.AddHeadersToTheRequest
            (
                jwtToken,
                requestMessage,
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ClientIP, userIp),
                new KeyValuePair<string, string>(requestHeaderOptions.Value.ApiKey, requestConfigurationProxyService.GetApiKey())
            );

            response = await httpClient.SendAsync(requestMessage, cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<List<DTO_UserSearched>>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult ?? new Util_GenericResponse<List<DTO_UserSearched>>();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in SearchUserByUserName.");

            return new Util_GenericResponse<List<DTO_UserSearched>>
            {
                Errors = null,
                Message = "Something went wrong",
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = null
            };
        }
    }

    public async Task<Util_GenericResponse<byte[]>>
    UploadProfilePicture
    (
        string userId,
        string userIp,
        string jwtToken,
        string fogeryToken,
        string aspNetForgeryToken,
        IFormFile image
    )
    {
        HttpResponseMessage response = new();

        try
        {
            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userId), userId),
                (nameof(userIp), userIp),
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

            var fileContent = new StreamContent(image.OpenReadStream());
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(image.ContentType);

            var formData = new MultipartFormDataContent
            {
                { new StringContent(userId), "userId" },
                { fileContent, "image", image.FileName }
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteAccountManagmentForClient + Route_AccountManagmentRoute.UploadProfilePicture.Replace("{userId}", userId.ToString()))
            {
                Content = formData
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

            response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<byte[]>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in UploadProfilePicture.");

            return new Util_GenericResponse<byte[]>
            {
                Errors = null,
                Message = "Something went wrong",
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = null
            };
        }
    }
}