using MudBlazor;
using System.Net;
using SafeShare.Client.Shared.Forms;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using SafeShare.ClientDTO.AccountManagment;
using SafeShare.ClientUtilities.Responses;
using SafeShare.ClientServerShared.Routes;
using SafeShare.ClientServices.Interfaces;

namespace SafeShare.Client.Shared;

public partial class NavMenu
{
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private HttpClient httpClient { get; set; }
    [Inject] IDialogService DialogService { get; set; }
    [Inject] IClientService_UserManagment _userManagment { get; set; }

    MudMessageBox mbox { get; set; }

    protected override Task OnInitializedAsync()
    {
        Snackbar.Add("Reactor meltdown is imminent", Severity.Error);

        return base.OnInitializedAsync();
    }

    private void OpenDialog()
    {
        DialogOptions closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = false };

        DialogService.Show<Test1>("Simple Dialog", closeOnEscapeKey);
    }

    private async Task OnButtonClicked()
    {
        await mbox.Show();
        StateHasChanged();
    }

    private async Task CallTheApi()
    {
        var result = await _userManagment.CallTheApi();

        
    }
}