using System.Text;
using System.Text.Json;
using SafeShare.ClientDTO.Authentication;
using SafeShare.ClientServerShared.Routes;
using SafeShare.ClientUtilities.Responses;
using SafeShare.ClientServices.Interfaces;
using SafeShare.ClientDTO.AccountManagment;
using System.Security.Cryptography.X509Certificates;

namespace SafeShare.ClientServices.Authentication;

public class AuthenticationService(IHttpClientFactory httpClientFactory) : IAuthenticationService
{
    public async Task<ClientUtil_ApiResponse<bool>>
    RegisterUser
    (
        ClientDto_Register register
    )
    {
        try
        {
            var requestMessage = new HttpRequestMessage();
            var httpClient = httpClientFactory.CreateClient("MyHttpClient");

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

            var response = await httpClient.PostAsync(BaseRoute.RouteAuthenticationForClient + Route_AuthenticationRoute.Register, contentForm);

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


    public async Task<ClientUtil_ApiResponse<ClientDto_LoginResult>>
    LogInUser
    (
        ClientDto_Login login
    )
    {
        try
        {
            var requestMessage = new HttpRequestMessage();
            var httpClient = httpClientFactory.CreateClient("MyHttpClient");

            var loginData = new Dictionary<string, string>
            {
                { nameof(ClientDto_Login.Email), login.Email },
                { nameof(ClientDto_Login.Password), login.Password }
            };

            var contentForm = new FormUrlEncodedContent(loginData);

            var response = await httpClient.PostAsync(BaseRoute.RouteAuthenticationForClient + Route_AuthenticationRoute.Login, contentForm);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<ClientDto_LoginResult>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return readResult!;
        }
        catch (Exception)
        {
            return new ClientUtil_ApiResponse<ClientDto_LoginResult>()
            {
                Message = "Something went wrong",
                Errors = null,
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Succsess = false,
                Value = null
            };
        }
    }

    public async Task 
    LogoutUser
    (
        Guid userId
    )
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient("MyHttpClient");

            var content = new StringContent(JsonSerializer.Serialize(new { userId }), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(BaseRoute.RouteAuthenticationForClient + Route_AuthenticationRoute.LogOut.Replace("{userId}", userId.ToString()), content);

        }
        catch (Exception)
        {
        }
    }

    public async Task<ClientUtil_ApiResponse<bool>>
    ConfirmUserRegistration
    (
        ClientDto_ConfirmRegistration confirmRegistration
    )
    {
        try
        {
            var requestMessage = new HttpRequestMessage();
            var httpClient = httpClientFactory.CreateClient("MyHttpClient");

            var json = JsonSerializer.Serialize(confirmRegistration);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(BaseRoute.RouteAuthenticationForClient + Route_AuthenticationRoute.ConfirmRegistration, content);

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
    ReConfirmRegistrationRequest
    (
        ClientDto_ReConfirmRegistration ConfirmRegistration
    )
    {
        try
        {
            var requestMessage = new HttpRequestMessage();
            var httpClient = httpClientFactory.CreateClient("MyHttpClient");

            var json = JsonSerializer.Serialize(ConfirmRegistration);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(BaseRoute.RouteAuthenticationForClient + Route_AuthenticationRoute.ReConfirmRegistrationRequest, content);

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

    public async Task<ClientUtil_ApiResponse<ClientDto_LoginResult>>
    ConfirmLogin2FA
    (
        ClientDto_2FA TwoFA
    )
    {
        try
        {
            var requestMessage = new HttpRequestMessage();
            var httpClient = httpClientFactory.CreateClient("MyHttpClient");

            var json = JsonSerializer.Serialize(TwoFA);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(BaseRoute.RouteAuthenticationForClient + Route_AuthenticationRoute.ConfirmLogin.Replace("{userId}", TwoFA.UserId.ToString()), content);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<ClientDto_LoginResult>>(responseContent, new JsonSerializerOptions
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