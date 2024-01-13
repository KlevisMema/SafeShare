using MudBlazor;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientServices.Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace SafeShare.Client.Helpers;

internal class TokenExpiryHandler
(
    IClientAuthentication_TokenRefreshService tokenRefreshService,
    NavigationManager navigationManager,
    ILocalStorageService localStorage
) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage>
    SendAsync
    (
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var tokenRefreshed = await tokenRefreshService.RefreshToken();

            if (tokenRefreshed)
                return await base.SendAsync(request, cancellationToken);
            else
            {
                await localStorage.SetItemAsync<bool>("SessionExpired", true, cancellationToken);
                navigationManager.NavigateTo("/Login");
            }
        }

        return response;
    }
}