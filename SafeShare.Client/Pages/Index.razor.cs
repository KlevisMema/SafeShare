using System.Net.Http.Json;
using Blazored.LocalStorage;
using SafeShare.Client.Shared;
using SafeShare.Client.Helpers;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientDTO.Authentication;

namespace SafeShare.Client.Pages;

public partial class Index
{
    [CascadingParameter]
    public MainLayout? Layout { get; set; }


    [Inject] private IHttpClientFactory _clientFactory { get; set; }
    [Inject] private AppState AppState { get; set; }
    [Inject] private ILocalStorageService _localStorage { get; set; }
}