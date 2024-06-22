using System.Net;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using Microsoft.Net.Http.Headers;
using SafeShare.ProxyApi.Helpers;
using System.Collections.Specialized;
using SafeShare.ClientServerShared.Routes;
using SafeShare.ProxyApi.Container.Interfaces;
using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.DataTransormObject.SafeShareApi.Security;
using SafeShare.DataTransormObject.SafeShareApi.Authentication;
using Microsoft.Extensions.Options;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment;

namespace SafeShare.ProxyApi.Container.Services;

public class ProxyAuthentication
(
    ILogger<ProxyAuthentication> logger,
    IHttpClientFactory httpClientFactory,
    IOptions<API_Helper_RequestHeaderSettings> requestHeaderOptions,
    IRequestConfigurationProxyService requestConfigurationProxyService
) : IProxyAuthentication
{
    private const string Client = "ProxyHttpClient";
    private readonly string ApiKey = Environment.GetEnvironmentVariable("SAFE_SHARE_API_KEY") ?? string.Empty;

    public async Task<Util_GenericResponse<bool>>
    RegisterUser
    (
        DTO_Register register
    )
    {
        try
        {
            var httpClient = API_Helper_HttpClient.CreateClientInstance(requestConfigurationProxyService.GetClient(), httpClientFactory);

            var registerData = new Dictionary<string, string>
            {
                { nameof(DTO_Register.FullName), register.FullName },
                { nameof(DTO_Register.UserName), register.UserName },
                { nameof(DTO_Register.Email), register.Email },
                { nameof(DTO_Register.Gender), register.Gender.ToString() },
                { nameof(DTO_Register.Birthday), register.Birthday.ToString() },
                { nameof(DTO_Register.PhoneNumber), register.PhoneNumber },
                { nameof(DTO_Register.Password), register.Password },
                { nameof(DTO_Register.ConfirmPassword), register.ConfirmPassword },
                { nameof(DTO_Register.Enable2FA), register.Enable2FA.ToString() },
            };

            var contentForm = new FormUrlEncodedContent(registerData);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteAuthenticationForClient + Route_AuthenticationRoute.Register)
            {
                Content = contentForm
            };

            requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

            var response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
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

    public async Task<Util_GenericResponse<bool>>
    ConfirmRegistration
    (
         DTO_ConfirmRegistration confirmRegistrationDto
    )
    {
        try
        {
            var httpClient = API_Helper_HttpClient.CreateClientInstance(requestConfigurationProxyService.GetClient(), httpClientFactory);

            var json = JsonSerializer.Serialize(confirmRegistrationDto);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteAuthenticationForClient + Route_AuthenticationRoute.ConfirmRegistration)
            {
                Content = content
            };

            requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

            var response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
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

    public async Task<Util_GenericResponse<bool>>
    ReConfirmRegistrationRequest
    (
        DTO_ReConfirmRegistration ReConfirmRegistration
    )
    {
        try
        {
            var httpClient = API_Helper_HttpClient.CreateClientInstance(requestConfigurationProxyService.GetClient(), httpClientFactory);

            var json = JsonSerializer.Serialize(ReConfirmRegistration);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteAuthenticationForClient + Route_AuthenticationRoute.ReConfirmRegistrationRequest)
            {
                Content = content
            };

            requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

            var response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
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

    public async Task<Tuple<Util_GenericResponse<DTO_LoginResult>, HttpResponseMessage>>
    LogIn
    (
        DTO_Login loginDto
    )
    {
        try
        {
            var httpClient = API_Helper_HttpClient.CreateClientInstance(requestConfigurationProxyService.GetClient(), httpClientFactory);

            var loginData = new Dictionary<string, string>
            {
                { nameof(DTO_Login.Email), loginDto.Email },
                { nameof(DTO_Login.Password), loginDto.Password }
            };

            var contentForm = new FormUrlEncodedContent(loginData);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteAuthenticationForClient + Route_AuthenticationRoute.Login)
            {
                Content = contentForm
            };

            requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

            var response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<DTO_LoginResult>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return Tuple.Create(readResult!, response);
        }
        catch (Exception)
        {
            return Tuple.Create(
            new Util_GenericResponse<DTO_LoginResult>()
            {
                Message = "Something went wrong",
                Errors = null,
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Succsess = false,
                Value = null
            }, new HttpResponseMessage());
        }
    }

    public async Task<Util_GenericResponse<string>>
    SaveUserPublicKey
    (
        string userId,
        string userIp,
        string jwtToken,
        string publicKey
    )
    {
        try
        {
            API_Helper_ParamsStringChecking.CheckNullOrEmpty
            (
                (nameof(userId), userId),
                (nameof(userIp), userIp),
                (nameof(jwtToken), jwtToken),
                (nameof(publicKey), publicKey)
            );

            var httpClient = API_Helper_HttpClient.CreateClientInstance(requestConfigurationProxyService.GetClient(), httpClientFactory);

            var content = new StringContent(JsonSerializer.Serialize(publicKey), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteAuthenticationForClient + Route_AuthenticationRoute.SaveUserPublicKey.Replace("{userId}", userId))
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

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<string>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Exception in SaveUserPublicKey.");

            return new Util_GenericResponse<string>()
            {
                Message = "Something went wrong",
                Errors = null,
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Succsess = false,
                Value = null
            };
        }
    }

    public async Task<Tuple<Util_GenericResponse<DTO_LoginResult>, HttpResponseMessage>>
    ConfirmLogin2FA
    (
        Guid userId,
        string jwtToken,
        DTO_ConfirmLogin confirmLogin
    )
    {
        try
        {
            var httpClient = API_Helper_HttpClient.CreateClientInstance(requestConfigurationProxyService.GetClient(), httpClientFactory);

            var json = JsonSerializer.Serialize(confirmLogin);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteAuthenticationForClient + Route_AuthenticationRoute.ConfirmLogin.Replace("{userId}", userId.ToString()))
            {
                Content = content
            };

            requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");
            httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<DTO_LoginResult>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return Tuple.Create(readResult!, response);
        }
        catch (Exception)
        {
            return Tuple.Create(new Util_GenericResponse<DTO_LoginResult>()
            {
                Errors = null,
                Message = "Something went wrong, please try logging in again",
                Succsess = false,
                Value = null,
                StatusCode = System.Net.HttpStatusCode.InternalServerError
            }, new HttpResponseMessage());
        }
    }

    public async Task<HttpResponseMessage>
    LogoutUser
    (
        Guid userId,
        string jwtToken
    )
    {
        try
        {
            var httpClient = API_Helper_HttpClient.CreateClientInstance(requestConfigurationProxyService.GetClient(), httpClientFactory);

            var content = new StringContent(JsonSerializer.Serialize(new { userId }), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteAuthenticationForClient + Route_AuthenticationRoute.LogOut.Replace("{userId}", userId.ToString()))
            {
                Content = content
            };

            requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

            httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await httpClient.SendAsync(requestMessage);

            return response;
        }
        catch (Exception)
        {
            return new HttpResponseMessage();
        }
    }

    public async Task<Tuple<Util_GenericResponse<DTO_Token>, HttpResponseMessage>>
    RefreshToken
    (
        string jwtToken,
        string refreshToken,
        string refreshTokenId
    )
    {
        try
        {
            var httpClient = API_Helper_HttpClient.CreateClientInstance(requestConfigurationProxyService.GetClient(), httpClientFactory);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteAuthenticationForClient + Route_AuthenticationRoute.RefreshToken)
            {
                Content = null
            };

            requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

            requestMessage.Headers.Add("Cookie", $"AuthToken={jwtToken}; RefreshAuthToken={refreshToken}; RefreshAuthTokenId={refreshTokenId}");

            var response = await httpClient.SendAsync(requestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<Util_GenericResponse<DTO_Token>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return Tuple.Create(readResult!, response);
        }
        catch
        {
            return Tuple.Create(new Util_GenericResponse<DTO_Token>()
            {
                Errors = null,
                Message = "Something went wrong, please try logging in again",
                Succsess = false,
                Value = null,
                StatusCode = System.Net.HttpStatusCode.InternalServerError
            }, new HttpResponseMessage());
        }

    }
}