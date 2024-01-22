using System.Text;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using SafeShare.ClientDTO.Authentication;
using SafeShare.ClientServerShared.Routes;
using SafeShare.ClientUtilities.Responses;
using SafeShare.ClientServices.Interfaces;
using SafeShare.ClientDTO.AccountManagment;

namespace SafeShare.ClientServices.Authentication;

public class AuthenticationService(IHttpClientFactory httpClientFactory) : IAuthenticationService
{
    private const string Client = "MyHttpClient";

    public async Task<ClientUtil_ApiResponse<bool>>
    RegisterUser
    (
         ClientDto_Register register
    )
    {
        HttpResponseMessage response = new();

        try
        {
            var requestMessage = new HttpRequestMessage();
            var httpClient = httpClientFactory.CreateClient(Client);

            var registerData = new Dictionary<string, string>
            {
                { nameof(ClientDto_Register.FullName), register.FullName },
                { nameof(ClientDto_Register.Username), register.Username },
                { nameof(ClientDto_Register.Email), register.Email },
                { nameof(ClientDto_Register.Gender), register.Gender.ToString() },
                { nameof(ClientDto_Register.Birthday), register.Birthday.ToString() },
                { nameof(ClientDto_Register.PhoneNumber), register.PhoneNumber },
                { nameof(ClientDto_Register.Password), register.Password },
                { nameof(ClientDto_Register.ConfirmPassword), register.ConfirmPassword },
                { nameof(ClientDto_Register.TwoFA), register.TwoFA.ToString() },
            };

            var contentForm = new FormUrlEncodedContent(registerData);

            response = await httpClient.PostAsync(BaseRoute.RouteAuthenticationProxy + Route_AuthenticationRoute.Register, contentForm);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;

        }
        catch (Exception)
        {
            return new ClientUtil_ApiResponse<bool>
            {
                Errors = null,
                Message = "Something went wrong, could not register!",
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = false
            };
        }
    }

    public async Task<ClientUtil_ApiResponse<ClientDto_LoginResult>>
    LogInUser
    (
        ClientDto_Login login
    )
    {
        HttpResponseMessage response = new();

        try
        {
            var requestMessage = new HttpRequestMessage();
            var httpClient = httpClientFactory.CreateClient(Client);

            var loginData = new Dictionary<string, string>
            {
                { nameof(ClientDto_Login.Email), login.Email },
                { nameof(ClientDto_Login.Password), login.Password }
            };

            var contentForm = new FormUrlEncodedContent(loginData);

            response = await httpClient.PostAsync(BaseRoute.RouteAuthenticationProxy + Route_AuthenticationRoute.Login, contentForm);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<ClientDto_LoginResult>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception ex)
        {
            return new ClientUtil_ApiResponse<ClientDto_LoginResult>()
            {
                Message = "Something went wrong, could not login!",
                Errors = null,
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = null
            };
        }
    }

    public async Task
    LogoutUser
    (

    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

        await httpClient.PostAsync(BaseRoute.RouteAuthenticationProxy + Route_AuthenticationRoute.LogOut, null);
    }

    public async Task<string>
    GetJwtToken()
    {
        HttpResponseMessage response = new();

        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            response = await httpClient.GetAsync(BaseRoute.RouteAuthenticationProxy + Route_AuthenticationRoute.JwtToken);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<string>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public async Task<ClientUtil_ApiResponse<bool>>
    ConfirmUserRegistration
    (
        ClientDto_ConfirmRegistration confirmRegistration
    )
    {
        HttpResponseMessage response = new();

        try
        {
            var requestMessage = new HttpRequestMessage();

            var httpClient = httpClientFactory.CreateClient(Client);

            var json = JsonSerializer.Serialize(confirmRegistration);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            response = await httpClient.PostAsync(BaseRoute.RouteAuthenticationProxy + Route_AuthenticationRoute.ConfirmRegistration, content);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception)
        {
            return new ClientUtil_ApiResponse<bool>
            {
                Errors = null,
                Message = "Something went wrong, could not confirm login",
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = false
            };
        }

    }

    public async Task<ClientUtil_ApiResponse<bool>>
    ReConfirmRegistrationRequest
    (
        ClientDto_ReConfirmRegistration ConfirmRegistration
    )
    {
        HttpResponseMessage response = new();

        try
        {
            var requestMessage = new HttpRequestMessage();
            var httpClient = httpClientFactory.CreateClient(Client);

            var json = JsonSerializer.Serialize(ConfirmRegistration);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            response = await httpClient.PostAsync(BaseRoute.RouteAuthenticationProxy + Route_AuthenticationRoute.ReConfirmRegistrationRequest, content);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception)
        {
            return new ClientUtil_ApiResponse<bool>
            {
                Errors = null,
                Message = "Something went wrong, could not confirm re registration proccess",
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = false
            };
        }
    }

    public async Task<ClientUtil_ApiResponse<ClientDto_LoginResult>>
    ConfirmLogin2FA
    (
        ClientDto_2FA TwoFA
    )
    {
        HttpResponseMessage response = new();

        try
        {
            var requestMessage = new HttpRequestMessage();

            var httpClient = httpClientFactory.CreateClient(Client);

            var json = JsonSerializer.Serialize(TwoFA);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            response = await httpClient.PostAsync(BaseRoute.RouteAuthenticationProxy + Route_AuthenticationRoute.ConfirmLogin.Replace("{userId}", TwoFA.UserId.ToString()), content);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<ClientDto_LoginResult>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ArgumentNullException("Failed to deserialize the server response. The content may not match the expected format.");

            return readResult;
        }
        catch (Exception)
        {
            return new ClientUtil_ApiResponse<ClientDto_LoginResult>()
            {
                Errors = null,
                Message = "Something went wrong, please try logging in again",
                Succsess = false,
                Value = null,
                StatusCode = response.StatusCode
            };
        }
    }
}