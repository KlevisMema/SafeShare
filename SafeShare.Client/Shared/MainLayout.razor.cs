using MudBlazor;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientServices.Interfaces;


namespace SafeShare.Client.Shared;

public partial class MainLayout
{
    [Inject] private ISnackbar _snackbar { get; set; } = null!;
    [Inject] private IAuthenticationService authService { get; set; } = null!;
    [Inject] private ILocalStorageService _localStorage { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;

    private bool _drawerOpen = true;

    void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    async Task LogoutUser()
    {
        _snackbar.Add("Logging you out", Severity.Success, config => { config.CloseAfterNavigation = true; });
        await Task.Delay(2000);
        var userId = await _localStorage.GetItemAsStringAsync("UserData");
        await authService.LogoutUser(Guid.Parse(userId));
        await _localStorage.RemoveItemAsync("UserData");
        _navigationManager.NavigateTo("/Login");
    }
}