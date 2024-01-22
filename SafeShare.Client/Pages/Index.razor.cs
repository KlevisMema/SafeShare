using System.Net.Http.Json;
using Blazored.LocalStorage;
using SafeShare.Client.Shared;
using SafeShare.Client.Internal;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientDTO.Authentication;

namespace SafeShare.Client.Pages;

public partial class Index
{
    [CascadingParameter]
    public MainLayout? Layout { get; set; }

    [Inject] private SignalRService _signalR { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await _signalR.StartConnectionAsync();
    }
}