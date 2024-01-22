using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using SafeShare.ClientServices.Interfaces;

namespace SafeShare.Client.Internal;

public class SignalRService(IAuthenticationService authenticationService)
{
    private HubConnection? _hubConnection;
    public HubConnection? HubConnection => _hubConnection;

    public async Task
    StartConnectionAsync()
    {
        _hubConnection = new HubConnectionBuilder().WithUrl("https://localhost:7280/notifications", options =>
        {
            options.AccessTokenProvider = async () => await GetJwtToken();
        }).Build();

        await _hubConnection.StartAsync();
    }

    public async Task
    DisconnectAsync()
    {
        if (_hubConnection != null) 
            await _hubConnection.StopAsync();
    }

    public async ValueTask
    DisposeAsync()
    {
        if (_hubConnection != null)
            await _hubConnection.DisposeAsync();
    }

    private async Task<string>
    GetJwtToken()
    {
        string jwtToken = await authenticationService.GetJwtToken();

        return jwtToken;
    }
}