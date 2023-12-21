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
        var userId = Helper_GetUserId.GetUserIdDirectlyFromJwtToken(jwtToken);

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
}