﻿using System.Text.Json;
using SafeShare.ClientDTO.Authentication;
using SafeShare.ClientServerShared.Routes;
using SafeShare.ClientUtilities.Responses;
using SafeShare.ClientDTO.AccountManagment;
using SafeShare.ClientServices.Interfaces;
using System.Text;

namespace SafeShare.ClientServices.Authentication;

public class AuthenticationService(IHttpClientFactory httpClientFactory) : IAuthenticationService
{
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
}