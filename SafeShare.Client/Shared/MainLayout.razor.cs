using MudBlazor;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientDTO.GroupManagment;
using SafeShare.ClientServices.Interfaces;
using SafeShare.ClientServices.GroupManagment;


namespace SafeShare.Client.Shared;

public partial class MainLayout
{
    [Inject] private ISnackbar _snackbar { get; set; } = null!;
    [Inject] private IAuthenticationService authService { get; set; } = null!;
    [Inject] private ILocalStorageService _localStorage { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] public IClientService_GroupManagment _clientService_GroupManagment { get; set; } = null!;

    private bool _drawerOpen = true;


    public ClientDto_GroupTypes? GroupTypes { get; set; } = new();

    public string name = "klevis";
    public string LogOutText { get; set; } = "Log Out";

    public bool DataRetrieved { get; set; } = false;

    private bool hideLogOutBtn { get; set; } = false;

    protected override async Task
    OnInitializedAsync()
    {
        var getGroupTypes = await _clientService_GroupManagment.GetGroupTypes();

        if (getGroupTypes.Succsess && getGroupTypes.Value is not null)
        {
            GroupTypes = getGroupTypes.Value;
            DataRetrieved = true;
        }
    }

    private async Task
    LogoutUser()
    {
        LogOutText = "Logging Out";
        hideLogOutBtn = true;
        _snackbar.Add("Logging you out", Severity.Success, config => { config.CloseAfterNavigation = true; });
        await Task.Delay(2000);
        var userId = await _localStorage.GetItemAsStringAsync("UserData");
        await authService.LogoutUser(Guid.Parse(userId));
        await _localStorage.RemoveItemAsync("UserData");
        _navigationManager.NavigateTo("/");
        hideLogOutBtn = false;
        LogOutText = "Log Out";
    }

    private void
    DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }
}