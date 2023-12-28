using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using SafeShare.ClientDTO.Expense;
using SafeShare.ClientDTO.GroupManagment;
using SafeShare.ClientUtilities.Responses;
using SafeShare.ClientServerShared.Routes;
using SafeShare.ClientServices.Interfaces;
using SafeShare.ClientDTO.AccountManagment;

namespace SafeShare.ClientServices.ExpenseManagment;

public class ClientService_ExpenseManagment(IHttpClientFactory httpClientFactory) : IClientService_ExpenseManagment
{
    private const string Client = "MyHttpClient";

    public async Task<ClientUtil_ApiResponse<List<ClientDto_Expense>>>
    GetAllExpensesOfGroup
    (
        Guid groupId
    )
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var content = new StringContent(JsonSerializer.Serialize(new { groupId }), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Get, BaseRoute.RouteExpenseManagmentProxy + Route_ExpenseManagment.ProxyGetAllExpensesOfGroup)
            {
                Content = content
            };

            var response = await httpClient.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<List<ClientDto_Expense>>>(responseContent, new JsonSerializerOptions
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

    public async Task<ClientUtil_ApiResponse<ClientDto_Expense>>
    GetExpense
    (
        Guid groupId
    )
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var content = new StringContent(JsonSerializer.Serialize(new { groupId }), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Get, BaseRoute.RouteExpenseManagmentProxy + Route_ExpenseManagment.ProxyGetExpense)
            {
                Content = content
            };

            var response = await httpClient.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<ClientDto_Expense>>(responseContent, new JsonSerializerOptions
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

    public async Task<ClientUtil_ApiResponse<ClientDto_Expense>>
    CreateExpense
    (
        ClientDto_ExpenseCreate clientDto_ExpenseCreate
    )
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var registerData = new Dictionary<string, string>
            {
                { nameof(ClientDto_ExpenseCreate.GroupId), clientDto_ExpenseCreate.GroupId.ToString() },
                { nameof(ClientDto_ExpenseCreate.Title), clientDto_ExpenseCreate.Title.ToString() },
                { nameof(ClientDto_ExpenseCreate.Date), clientDto_ExpenseCreate.Date.ToString() },
                { nameof(ClientDto_ExpenseCreate.Amount), clientDto_ExpenseCreate.Amount.ToString() },
                { nameof(ClientDto_ExpenseCreate.Description), clientDto_ExpenseCreate.Description.ToString() },
                { nameof(ClientDto_ExpenseCreate.DecryptedAmount), clientDto_ExpenseCreate.DecryptedAmount.ToString() },
            };

            var contentForm = new FormUrlEncodedContent(registerData);

            var response = await httpClient.PostAsync(BaseRoute.RouteExpenseManagmentProxy + Route_ExpenseManagment.ProxyCreateExpense, contentForm);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<ClientDto_Expense>>(responseContent, new JsonSerializerOptions
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

    public async Task<ClientUtil_ApiResponse<ClientDto_Expense>>
    EditExpense
    (
        ClientDto_ExpenseCreate clientDto_ExpenseCreate
    )
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var registerData = new Dictionary<string, string>
            {
                { nameof(ClientDto_ExpenseCreate.GroupId), clientDto_ExpenseCreate.GroupId.ToString() },
                { nameof(ClientDto_ExpenseCreate.Title), clientDto_ExpenseCreate.Title.ToString() },
                { nameof(ClientDto_ExpenseCreate.Date), clientDto_ExpenseCreate.Date.ToString() },
                { nameof(ClientDto_ExpenseCreate.Amount), clientDto_ExpenseCreate.Amount.ToString() },
                { nameof(ClientDto_ExpenseCreate.Description), clientDto_ExpenseCreate.Description.ToString() },
                { nameof(ClientDto_ExpenseCreate.DecryptedAmount), clientDto_ExpenseCreate.DecryptedAmount.ToString() },
            };

            var contentForm = new FormUrlEncodedContent(registerData);

            var response = await httpClient.PutAsync(BaseRoute.RouteExpenseManagmentProxy + Route_ExpenseManagment.ProxyEditExpense, contentForm);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<ClientDto_Expense>>(responseContent, new JsonSerializerOptions
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
    DeleteExpense
    (
        ClientDto_ExpenseDelete clientDto_ExpenseDelete
    )
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var content = new StringContent(JsonSerializer.Serialize(new { clientDto_ExpenseDelete }), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Delete, BaseRoute.RouteExpenseManagmentProxy + Route_ExpenseManagment.ProxyDeleteExpense)
            {
                Content = content
            };

            var response = await httpClient.SendAsync(request);

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
}