using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using SafeShare.ClientDTO.Expense;
using SafeShare.ClientDTO.GroupManagment;
using SafeShare.ClientDTO.Authentication;
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
        HttpResponseMessage response = new();

        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var content = new StringContent(JsonSerializer.Serialize(groupId), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Get, BaseRoute.RouteExpenseManagmentProxy + Route_ExpenseManagment.ProxyGetAllExpensesOfGroup + $"?groupId={groupId}");

            response = await httpClient.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<List<ClientDto_Expense>>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return readResult!;
        }
        catch (Exception)
        {
            return new ClientUtil_ApiResponse<List<ClientDto_Expense>>()
            {
                Message = "Something went wrong, expenses were not retrieved",
                Errors = null,
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = null
            };
        }
    }

    public async Task<ClientUtil_ApiResponse<ClientDto_Expense>>
    GetExpense
    (
        Guid groupId
    )
    {
        HttpResponseMessage response = new();

        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var content = new StringContent(JsonSerializer.Serialize(new { groupId }), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Get, BaseRoute.RouteExpenseManagmentProxy + Route_ExpenseManagment.ProxyGetExpense)
            {
                Content = content
            };

            response = await httpClient.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<ClientDto_Expense>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return readResult!;
        }
        catch (Exception)
        {
            return new ClientUtil_ApiResponse<ClientDto_Expense>()
            {
                Message = "Something went wrong, expense was not retrieved",
                Errors = null,
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = null
            };
        }
    }

    public async Task<ClientUtil_ApiResponse<ClientDto_Expense>>
    CreateExpense
    (
        ClientDto_ExpenseCreate clientDto_ExpenseCreate
    )
    {
        HttpResponseMessage response = new();

        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var registerData = new Dictionary<string, string>
            {
                { nameof(ClientDto_ExpenseCreate.GroupId), clientDto_ExpenseCreate.GroupId.ToString() },
                { nameof(ClientDto_ExpenseCreate.Title), clientDto_ExpenseCreate.Title.ToString() },
                { nameof(ClientDto_ExpenseCreate.Date), clientDto_ExpenseCreate.Date.ToString() },
                { nameof(ClientDto_ExpenseCreate.Amount), clientDto_ExpenseCreate.Amount.ToString() },
                { nameof(ClientDto_ExpenseCreate.Description), clientDto_ExpenseCreate.Description.ToString() }
            };

            var contentForm = new FormUrlEncodedContent(registerData);

            response = await httpClient.PostAsync(BaseRoute.RouteExpenseManagmentProxy + Route_ExpenseManagment.ProxyCreateExpense, contentForm);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<ClientDto_Expense>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return readResult!;
        }
        catch (Exception)
        {
            return new ClientUtil_ApiResponse<ClientDto_Expense>()
            {
                Message = "Something went wrong, expense was not created!",
                Errors = null,
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = null
            };
        }
    }

    public async Task<ClientUtil_ApiResponse<ClientDto_Expense>>
    EditExpense
    (
        Guid expenseId,
        ClientDto_ExpenseCreate clientDto_ExpenseCreate
    )
    {
        HttpResponseMessage response = new();

        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var registerData = new Dictionary<string, string>
            {
                { nameof(ClientDto_ExpenseCreate.GroupId), clientDto_ExpenseCreate.GroupId.ToString() },
                { nameof(ClientDto_ExpenseCreate.Title), clientDto_ExpenseCreate.Title.ToString() },
                { nameof(ClientDto_ExpenseCreate.Date), clientDto_ExpenseCreate.Date.ToString() },
                { nameof(ClientDto_ExpenseCreate.Amount), clientDto_ExpenseCreate.Amount.ToString() },
                { nameof(ClientDto_ExpenseCreate.Description), clientDto_ExpenseCreate.Description.ToString() }
            };

            var contentForm = new FormUrlEncodedContent(registerData);

            response = await httpClient.PutAsync(BaseRoute.RouteExpenseManagmentProxy + Route_ExpenseManagment.ProxyEditExpense + $"?expenseId={expenseId}", contentForm);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<ClientDto_Expense>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return readResult!;
        }
        catch (Exception)
        {
            return new ClientUtil_ApiResponse<ClientDto_Expense>()
            {
                Message = "Something went wrong, expense was not edited",
                Errors = null,
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = null
            };
        }
    }

    public async Task<ClientUtil_ApiResponse<bool>>
    DeleteExpense
    (
        ClientDto_ExpenseDelete clientDto_ExpenseDelete
    )
    {
        HttpResponseMessage response = new();

        try
        {
            var httpClient = httpClientFactory.CreateClient(Client);

            var content = new StringContent(JsonSerializer.Serialize(clientDto_ExpenseDelete), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Delete, BaseRoute.RouteExpenseManagmentProxy + Route_ExpenseManagment.ProxyDeleteExpense)
            {
                Content = content
            };

             response = await httpClient.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();

            var readResult = JsonSerializer.Deserialize<ClientUtil_ApiResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return readResult!;
        }
        catch (Exception)
        {
            return new ClientUtil_ApiResponse<bool>()
            {
                Message = "Something went wrong, expense was not deleted",
                Errors = null,
                StatusCode = response.StatusCode,
                Succsess = false,
                Value = false
            };
        }
    }
}