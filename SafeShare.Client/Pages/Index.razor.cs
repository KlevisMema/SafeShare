using Blazored.LocalStorage;
using SafeShare.Client.Helpers;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientDTO.Authentication;
using System.Net.Http.Json;
using SafeShare.Client.Shared;

namespace SafeShare.Client.Pages;

public partial class Index
{
    [CascadingParameter]
    public MainLayout? Layout { get; set; }


    [Inject] private IHttpClientFactory _clientFactory { get; set; }
    [Inject] private AppState AppState { get; set; }
    [Inject] private ILocalStorageService _localStorage { get; set; }

    protected override async Task OnInitializedAsync()
    {

    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            //await _localStorage.SetItemAsync("Client", AppState.getClientSecrests());
        }
    }

    private async Task test()
    {
        try
        {
            var client = _clientFactory.CreateClient("MyHttpClient");

            var response = await client.GetAsync("api/Test/yyyy");

            var x = response.Content.ReadAsStringAsync();

            var y = response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {

            throw;
        }
    }
}