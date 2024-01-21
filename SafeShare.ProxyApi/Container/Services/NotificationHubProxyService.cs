using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SafeShare.ProxyApi.Container.Services;

[Authorize(AuthenticationSchemes = "Default")]
public class NotificationHubProxyService : Hub
{
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }
}