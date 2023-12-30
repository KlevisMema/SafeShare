using MudBlazor;
using System.Net;
using SafeShare.Client.Shared.Forms;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientDTO.GroupManagment;
using SafeShare.ClientUtilities.Responses;
using SafeShare.ClientServerShared.Routes;
using SafeShare.ClientServices.Interfaces;
using SafeShare.ClientDTO.AccountManagment;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting.Server;
using SafeShare.Client.Shared.Forms.Account;

namespace SafeShare.Client.Shared;

public partial class NavMenu
{
    [Parameter]
    public ClientDto_GroupTypes? GroupTypes { get; set; }
    [Parameter] 
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject] IDialogService DialogService { get; set; } = null!;
    [Inject] IClientService_UserManagment _userManagment { get; set; } = null!;

    MudMessageBox mbox { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        return base.OnInitializedAsync();
    }

    private async Task OnButtonClicked()
    {
        await mbox.Show();
        StateHasChanged();
    }

    private async Task OpenPopUpForm()
    {
        var options = new DialogOptions { ClassBackground = "my-custom-class" };
        var dialog = await DialogService.ShowAsync<RequestChangeEmailAddress>("Change Email Dialog", options);
        var result = await dialog.Result;
    }
}