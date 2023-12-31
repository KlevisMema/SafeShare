using MudBlazor;
using System.Net;
using SafeShare.Client.Helpers;
using Microsoft.Extensions.Options;
using SafeShare.Client.Shared.Forms;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientDTO.GroupManagment;
using SafeShare.ClientUtilities.Responses;
using SafeShare.ClientServerShared.Routes;
using SafeShare.ClientServices.Interfaces;
using SafeShare.Client.Shared.Forms.Group;
using Microsoft.AspNetCore.Hosting.Server;
using SafeShare.ClientDTO.AccountManagment;
using System.ComponentModel.DataAnnotations;
using SafeShare.Client.Shared.Forms.Account;

namespace SafeShare.Client.Shared;

public partial class NavMenu
{
    [Parameter] public ClientDto_GroupTypes? GroupTypes { get; set; }
    [Parameter] public ISnackbar Snackbar { get; set; } = null!;
    [Parameter] public bool DataRetrieved { get; set; }
    [Inject] private AppState _appState { get; set; } = null!;
    [Inject] IDialogService DialogService { get; set; } = null!;

    protected override Task
    OnInitializedAsync()
    {
        _appState.OnNewGroupCreated += HandleNewGroupCreated;
        return base.OnInitializedAsync();
    }

    private void HandleNewGroupCreated(ClientDto_GroupType? newGroup)
    {
        if (newGroup != null)
        {
            GroupTypes.GroupsCreated.Add(newGroup);
            StateHasChanged();
        }
    }
    public void Dispose()
    {
        _appState.OnNewGroupCreated -= HandleNewGroupCreated;
    }

    private async Task
    OpenPopUpDeactivateAccountForm()
    {
        var dialog = await DialogService.ShowAsync<DeactivateAccount>("Deactivate Account Dialog", DialogOptions());
        await dialog.Result;
    }

    private async Task
    OpenPopUpChangeEmailForm()
    {
        var dialog = await DialogService.ShowAsync<RequestChangeEmailAddress>("Change Email Dialog", DialogOptions());
        await dialog.Result;
    }

    private async Task
    OpenPopUpChangePasswordForm()
    {
        var dialog = await DialogService.ShowAsync<ChangePassword>("Change Password Dialog", DialogOptions());
        await dialog.Result;
    }

    private async Task
        OpenPopUpCreateGroup()
    {
        var dialog = await DialogService.ShowAsync<CreateGroup>("Change Email Dialog", DialogOptions());
        await dialog.Result;
    }

    private DialogOptions
    DialogOptions()
    {
        return new()
        {
            ClassBackground = "my-custom-class",
            CloseOnEscapeKey = false,
            DisableBackdropClick = true,
            CloseButton = true,
            Position = DialogPosition.Center
        };
    }
}