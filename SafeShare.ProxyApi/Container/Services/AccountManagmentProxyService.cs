using System.Text;
using System.Net.Http;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using SafeShare.ClientServerShared.Routes;
using SafeShare.ProxyApi.Container.Interfaces;
using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.DataTransormObject.SafeShareApi.UserManagment;
using SafeShare.DataTransormObject.SafeShareApi.Authentication;

namespace SafeShare.ProxyApi.Container.Services;

public class AccountManagmentProxyService(IHttpClientFactory httpClientFactory) : IAccountManagmentProxyService
{
    private const string Client = "ProxyHttpClient";
    private readonly string ApiKey = Environment.GetEnvironmentVariable("SAFE_SHARE_API_KEY") ?? string.Empty;

    public async Task<Util_GenericResponse<DTO_UserUpdatedInfo>>
    GetUser
    (
        string userId,
        string jwtToken
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

        var content = new StringContent(JsonSerializer.Serialize(new { userId }), Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, BaseRoute.RouteAccountManagmentForClient + Route_AccountManagmentRoute.GetUser.Replace("{userId}", userId.ToString()))
        {
            Content = content
        };

        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<DTO_UserUpdatedInfo>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return readResult ?? new Util_GenericResponse<DTO_UserUpdatedInfo>();
    }

    public async Task<Tuple<Util_GenericResponse<DTO_UserUpdatedInfo>, HttpResponseMessage>>
    UpdateUser
    (
        string userId,
        string jwtToken,
        DTO_UserInfo userInfo
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

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

        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<DTO_UserUpdatedInfo>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return Tuple.Create(readResult!, response) ?? Tuple.Create(
            new Util_GenericResponse<DTO_UserUpdatedInfo>()
            {
                Message = "Something went wrong",
                Errors = null,
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Succsess = false,
                Value = null
            }, new HttpResponseMessage()); ;
    }

    public async Task<Util_GenericResponse<bool>>
    ChangePassword
    (
        string userId,
        string jwtToken,
        DTO_UserChangePassword changePassword
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

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

        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return readResult ?? new Util_GenericResponse<bool>();
    }

    public async Task<Tuple<Util_GenericResponse<bool>, HttpResponseMessage>>
    DeactivateAccount
    (
        string userId,
        string jwtToken,
        DTO_DeactivateAccount deactivateAccount
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

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

        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return Tuple.Create(readResult ?? new Util_GenericResponse<bool>(), response);
    }

    public async Task<Util_GenericResponse<bool>>
    ActivateAccountRequest
    (
        string email
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

        var content = new StringContent(JsonSerializer.Serialize(new { email }), Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteAccountManagmentForClient + Route_AccountManagmentRoute.ActivateAccountRequest + $"?email={email}")
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

        return readResult ?? new Util_GenericResponse<bool>();
    }

    public async Task<Util_GenericResponse<bool>>
    ActivateAccountRequestConfirmation
    (
        DTO_ActivateAccountConfirmation activateAccountConfirmationDto
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

        var json = JsonSerializer.Serialize(activateAccountConfirmationDto);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteAccountManagmentForClient + Route_AccountManagmentRoute.ActivateAccountRequestConfirmation)
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

        return readResult ?? new Util_GenericResponse<bool>();
    }

    public async Task<Util_GenericResponse<bool>>
    ForgotPassword
    (
        DTO_ForgotPassword forgotPassword
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

        var registerData = new Dictionary<string, string>
        {
            { nameof(DTO_ForgotPassword.Email), forgotPassword.Email }
        };

        var contentForm = new FormUrlEncodedContent(registerData);

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteAccountManagmentForClient + Route_AccountManagmentRoute.ForgotPassword)
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

        return readResult ?? new Util_GenericResponse<bool>();
    }

    public async Task<Util_GenericResponse<bool>>
    ResetPassword
    (
        DTO_ResetPassword resetPassword
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

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

        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return readResult ?? new Util_GenericResponse<bool>();
    }

    public async Task<Util_GenericResponse<bool>>
    RequestChangeEmail
    (
        string userId,
        string jwtToken,
        DTO_ChangeEmailAddressRequest emailAddress
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

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

        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return readResult ?? new Util_GenericResponse<bool>();
    }

    public async Task<Tuple<Util_GenericResponse<bool>, HttpResponseMessage>>
    ConfirmChangeEmailAddressRequest
    (
        string userId,
        string jwtToken,
        DTO_ChangeEmailAddressRequestConfirm changeEmailAddressConfirmDto
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

        var json = JsonSerializer.Serialize(changeEmailAddressConfirmDto);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteAccountManagmentForClient + Route_AccountManagmentRoute.ConfirmChangeEmailRequest.Replace("{userId}", userId))
        {
            Content = content
        };

        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return Tuple.Create(readResult ?? new Util_GenericResponse<bool>(), response);
    }

    public async Task<Util_GenericResponse<List<DTO_UserSearched>>>
    SearchUserByUserName
    (
        string userId,
        string jwtToken,
        string userName,
        CancellationToken cancellationToken
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

        var content = new StringContent(JsonSerializer.Serialize(new { userName }), Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, BaseRoute.RouteAccountManagmentForClient + Route_AccountManagmentRoute.SearchUserByUserName.Replace("{userId}", userId.ToString()) + $"?username={userName}")
        {
            Content = content,

        };

        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage, cancellationToken);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<List<DTO_UserSearched>>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return readResult ?? new Util_GenericResponse<List<DTO_UserSearched>>();
    }

    public async Task<Util_GenericResponse<byte[]>>
    UploadProfilePicture
    (
        string userId,
        string jwtToken,
        IFormFile image
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

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

        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<byte[]>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return readResult ?? new Util_GenericResponse<byte[]>();
    }
}