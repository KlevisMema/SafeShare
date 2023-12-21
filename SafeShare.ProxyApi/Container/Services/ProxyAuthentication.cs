using System.Net;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using Microsoft.Net.Http.Headers;
using System.Collections.Specialized;
using SafeShare.ClientServerShared.Routes;
using SafeShare.ProxyApi.Container.Interfaces;
using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.DataTransormObject.SafeShareApi.Security;
using SafeShare.DataTransormObject.SafeShareApi.Authentication;

namespace SafeShare.ProxyApi.Container.Services;

public class ProxyAuthentication(IHttpClientFactory httpClientFactory) : IProxyAuthentication
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
            var httpClient = httpClientFactory.CreateClient(Client);

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
            var httpClient = httpClientFactory.CreateClient(Client);

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
            var httpClient = httpClientFactory.CreateClient(Client);

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
            var httpClient = httpClientFactory.CreateClient(Client);

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
            var httpClient = httpClientFactory.CreateClient(Client);

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
            var httpClient = httpClientFactory.CreateClient(Client);

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
            var httpClient = httpClientFactory.CreateClient(Client);

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