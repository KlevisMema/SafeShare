using System.Text;
using System.Net.Http;
using System.Text.Json;
using SafeShare.ClientServerShared.Routes;
using SafeShare.ClientUtilities.Responses;
using SafeShare.ClientServices.Interfaces;
using SafeShare.ClientDTO.AccountManagment;

namespace SafeShare.ClientServices.UserManagment;

public class ClientService_UserManagment(IHttpClientFactory httpClientFactory) : IClientService_UserManagment
{
    private const string Client = "MyHttpClient";

    public async Task<ClientUtil_ApiResponse<ClientDto_UserInfo>>
    GetUser()
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var response = await httpClient.GetAsync(BaseRoute.RouteAccountManagmentProxy + Route_AccountManagmentRoute.ProxyGetUser);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<ClientDto_UserInfo>>(responseContent, new JsonSerializerOptions
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

    public async Task<ClientUtil_ApiResponse<ClientDto_UserInfo>>
    UpdateUser
    (
        ClientDto_UpdateUser clientDto_UpdateUser
    )
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var updateUserData = new Dictionary<string, string>
            {
                { nameof(ClientDto_UpdateUser.FullName), clientDto_UpdateUser.FullName },
                { nameof(ClientDto_UpdateUser.Birthday), clientDto_UpdateUser.Birthday.ToString() },
                { nameof(ClientDto_UpdateUser.Gender), clientDto_UpdateUser.Gender.ToString() },
                { nameof(ClientDto_UpdateUser.UserName), clientDto_UpdateUser.UserName },
                { nameof(ClientDto_UpdateUser.PhoneNumber), clientDto_UpdateUser.PhoneNumber },
                { nameof(ClientDto_UpdateUser.Enable2FA), clientDto_UpdateUser.Enable2FA.ToString() }
            };

            var content = new FormUrlEncodedContent(updateUserData);

            var response = await httpClient.PutAsync(BaseRoute.RouteAccountManagmentProxy + Route_AccountManagmentRoute.ProxyUpdateUser, content);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<ClientDto_UserInfo>>(responseContent, new JsonSerializerOptions
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
    ChangePassword
    (
        ClientDto_UserChangePassword userChangePassword
    )
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var changePasswordData = new Dictionary<string, string>
            {
                { nameof(ClientDto_UserChangePassword.OldPassword), userChangePassword.OldPassword },
                { nameof(ClientDto_UserChangePassword.NewPassword), userChangePassword.NewPassword},
                { nameof(ClientDto_UserChangePassword.ConfirmNewPassword), userChangePassword.ConfirmNewPassword},
            };

            var content = new FormUrlEncodedContent(changePasswordData);

            var response = await httpClient.PutAsync(BaseRoute.RouteAccountManagmentProxy + Route_AccountManagmentRoute.ProxyChangePassword, content);

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
    DeactivateAccount
    (
        ClientDto_DeactivateAccount deactivateAccount
    )
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var deactivateAccountData = new Dictionary<string, string>
            {
                { nameof(ClientDto_DeactivateAccount.Email), deactivateAccount.Email },
                { nameof(ClientDto_DeactivateAccount.Password), deactivateAccount.Password}
            };

            var content = new FormUrlEncodedContent(deactivateAccountData);

            var response = await httpClient.PostAsync(BaseRoute.RouteAccountManagmentProxy + Route_AccountManagmentRoute.ProxyDeactivateAccount, content);

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
    ActivateAccountRequest()
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var response = await httpClient.PostAsync(BaseRoute.RouteAccountManagmentProxy + Route_AccountManagmentRoute.ActivateAccountRequest, null);

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
    ActivateAccountRequestConfirmation
    (
        ClientDto_ActivateAccountConfirmation activateAccountConfirmation
    )
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var json = JsonSerializer.Serialize(activateAccountConfirmation);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(BaseRoute.RouteAccountManagmentProxy + Route_AccountManagmentRoute.ActivateAccountRequestConfirmation, content);

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
    ForgotPassword
    (
        ClientDto_ForgotPassword forgotPassword
    )
    {
        try
        {
            var requestMessage = new HttpRequestMessage();
            var httpClient = httpClientFactory.CreateClient(Client);

            var forgotPasswordData = new Dictionary<string, string>
            {
                { nameof(ClientDto_ForgotPassword.Email), forgotPassword.Email }
            };

            var content = new FormUrlEncodedContent(forgotPasswordData);

            var response = await httpClient.PostAsync(BaseRoute.RouteAccountManagmentProxy + Route_AccountManagmentRoute.ForgotPassword, content);

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
    ResetPassword
    (
        ClientDto_ResetPassword resetPassword
    )
    {
        try
        {
            var requestMessage = new HttpRequestMessage();
            var httpClient = httpClientFactory.CreateClient(Client);

            var forgotPasswordData = new Dictionary<string, string>
            {
                { nameof(ClientDto_ResetPassword.Email), resetPassword.Email },
                { nameof(ClientDto_ResetPassword.Token), resetPassword.Token },
                { nameof(ClientDto_ResetPassword.NewPassword), resetPassword.NewPassword },
                { nameof(ClientDto_ResetPassword.ConfirmNewPassword), resetPassword.ConfirmNewPassword }
            };

            var content = new FormUrlEncodedContent(forgotPasswordData);

            var response = await httpClient.PostAsync(BaseRoute.RouteAccountManagmentProxy + Route_AccountManagmentRoute.ResetPassword, content);

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
    RequestChangeEmail
    (
        ClientDto_ChangeEmailAddressRequest changeEmailAddressRequest
    )
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var requestChangeEmailData = new Dictionary<string, string>
            {
                { nameof(ClientDto_ChangeEmailAddressRequest.ConfirmNewEmailAddress), changeEmailAddressRequest.ConfirmNewEmailAddress },
                { nameof(ClientDto_ChangeEmailAddressRequest.NewEmailAddress), changeEmailAddressRequest.NewEmailAddress },
                { nameof(ClientDto_ChangeEmailAddressRequest.CurrentEmailAddress), changeEmailAddressRequest.CurrentEmailAddress },
            };

            var content = new FormUrlEncodedContent(requestChangeEmailData);

            var response = await httpClient.PostAsync(BaseRoute.RouteAccountManagmentProxy + Route_AccountManagmentRoute.ProxyRequestChangeEmail, content);

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
    ConfirmChangeEmailAddressRequest
    (
        ClientDto_ChangeEmailAddressRequestConfirm changeEmailAddressRequestConfirm
    )
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var json = JsonSerializer.Serialize(changeEmailAddressRequestConfirm);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(BaseRoute.RouteAccountManagmentProxy + Route_AccountManagmentRoute.ProxyConfirmChangeEmailRequest, content);

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

    public async Task<ClientUtil_ApiResponse<List<ClientDto_UserSearched>>>
    SearchUserByUserName
    (
        string userName
    )
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var response = await httpClient.GetAsync(BaseRoute.RouteAccountManagmentProxy + Route_AccountManagmentRoute.ProxySearchUserByUserName + $"?username={userName}");

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<List<ClientDto_UserSearched>>>(responseContent, new JsonSerializerOptions
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