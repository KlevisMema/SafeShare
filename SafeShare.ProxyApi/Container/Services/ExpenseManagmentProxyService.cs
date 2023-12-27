using System.Text;
using System.Net.Http;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using SafeShare.ClientServerShared.Routes;
using SafeShare.ProxyApi.Container.Interfaces;
using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.DataAccessLayer.Models.SafeShareApi;
using SafeShare.DataAccessLayer.Models.SafeShareApiKey;
using SafeShare.DataTransormObject.SafeShareApi.UserManagment;
using SafeShare.DataTransormObject.SafeShareApi.ExpenseManagment;

namespace SafeShare.ProxyApi.Container.Services;

public class ExpenseManagmentProxyService(IHttpClientFactory httpClientFactory) : IExpenseManagmentProxyService
{
    private const string Client = "ProxyHttpClient";
    private readonly string ApiKey = Environment.GetEnvironmentVariable("SAFE_SHARE_API_KEY") ?? string.Empty;

    public async Task<Util_GenericResponse<List<DTO_Expense>>>
    GetAllExpensesOfGroup
    (
        string userId,
        string jwtToken,
        Guid groupId
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

        var content = new StringContent(JsonSerializer.Serialize(new { userId, groupId }), Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, BaseRoute.RouteExpenseManagmentForClient + Route_ExpenseManagment.GetAllExpensesOfGroup.Replace("{userId}", userId.ToString()))
        {
            Content = content
        };

        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<List<DTO_Expense>>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return readResult ?? new Util_GenericResponse<List<DTO_Expense>>();
    }

    public async Task<Util_GenericResponse<DTO_Expense>>
    GetExpense
    (
        string userId,
        string jwtToken,
        Guid expenseId
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

        var content = new StringContent(JsonSerializer.Serialize(new { expenseId, userId }), Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, BaseRoute.RouteExpenseManagmentForClient + Route_ExpenseManagment.GetExpense.Replace("{userId}", userId.ToString()))
        {
            Content = content
        };

        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<DTO_Expense>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return readResult ?? new Util_GenericResponse<DTO_Expense>();
    }

    public async Task<Util_GenericResponse<DTO_Expense>>
    CreateExpense
    (
        string userId,
        string jwtToken,
        DTO_ExpenseCreate expenseDto
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

        var expense = new Dictionary<string, string>
        {
            { nameof(DTO_ExpenseCreate.GroupId), expenseDto.GroupId.ToString() },
            { nameof(DTO_ExpenseCreate.Title), expenseDto.Title.ToString() },
            { nameof(DTO_ExpenseCreate.Date), expenseDto.Date.ToString() },
            { nameof(DTO_ExpenseCreate.Amount), expenseDto.Amount.ToString() },
            { nameof(DTO_ExpenseCreate.Description), expenseDto.Description.ToString() },
            { nameof(DTO_ExpenseCreate.DecryptedAmount), expenseDto.DecryptedAmount.ToString() },
        };

        var contentForm = new FormUrlEncodedContent(expense);

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, BaseRoute.RouteExpenseManagmentForClient + Route_ExpenseManagment.CreateExpense.Replace("{userId}", userId))
        {
            Content = contentForm
        };

        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<DTO_Expense>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return readResult ?? new Util_GenericResponse<DTO_Expense>();
    }

    public async Task<Util_GenericResponse<DTO_Expense>>
    EditExpense
    (
        string userId,
        string jwtToken,
        Guid expenseId,
        DTO_ExpenseCreate expenseCreateDto
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

        var expense = new Dictionary<string, string>
        {
            { nameof(DTO_ExpenseCreate.GroupId), expenseCreateDto.GroupId.ToString() },
            { nameof(DTO_ExpenseCreate.Title), expenseCreateDto.Title.ToString() },
            { nameof(DTO_ExpenseCreate.Date), expenseCreateDto.Date.ToString() },
            { nameof(DTO_ExpenseCreate.Amount), expenseCreateDto.Amount.ToString() },
            { nameof(DTO_ExpenseCreate.Description), expenseCreateDto.Description.ToString() },
            { nameof(DTO_ExpenseCreate.DecryptedAmount), expenseCreateDto.DecryptedAmount.ToString() },
        };

        var contentForm = new FormUrlEncodedContent(expense);

        var requestMessage = new HttpRequestMessage(HttpMethod.Put, BaseRoute.RouteExpenseManagmentForClient + Route_ExpenseManagment.EditExpense.Replace("{userId}" + $"?expenseId={expenseId}", userId))
        {
            Content = contentForm
        };

        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<DTO_Expense>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return readResult ?? new Util_GenericResponse<DTO_Expense>();
    }

    public async Task<Util_GenericResponse<bool>>
    DeleteExpense
    (
        string userId,
        string jwtToken,
        DTO_ExpenseDelete expenseDelete
    )
    {
        var httpClient = httpClientFactory.CreateClient(Client);

        var expense = new Dictionary<string, string>
        {
            { nameof(DTO_ExpenseDelete.ExpenseId), expenseDelete.ExpenseId.ToString() },
            { nameof(DTO_ExpenseDelete.UserId), expenseDelete.UserId.ToString() },
            { nameof(DTO_ExpenseDelete.ExpenseAmount), expenseDelete.ExpenseAmount.ToString() },
        };

        var contentForm = new FormUrlEncodedContent(expense);

        var requestMessage = new HttpRequestMessage(HttpMethod.Delete, BaseRoute.RouteExpenseManagmentForClient + Route_ExpenseManagment.DeleteExpense.Replace("{userId}", userId))
        {
            Content = contentForm
        };

        requestMessage.Headers.Add("X-Api-Key", $"{ApiKey}");

        httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();

        var readResult = JsonSerializer.Deserialize<Util_GenericResponse<bool>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return readResult ?? new Util_GenericResponse<bool>();
    }
}