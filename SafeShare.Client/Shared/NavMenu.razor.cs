using MudBlazor;
using System.Net;
using SafeShare.Client.Internal;
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
    [Inject] NavigationManager _navigationManager { get; set; } = null!;

    protected override Task
    OnInitializedAsync()
    {
        _appState.OnNewGroupCreated += HandleNewGroupCreated;
        _appState.OnGroupEdited += HandleGroupEdited;
        _appState.OnGroupDeleted += HandleGroupDeleted;
        _appState.OnRemovedFromGroup += HandleRemovedFromGroup;
        _appState.OnGroupInvitationAccepted += HandleGroupInvitationAccepted;
        return base.OnInitializedAsync();
    }

    private void
    HandleNewGroupCreated
    (
        ClientDto_GroupType? newGroup
    )
    {
        if (newGroup != null)
        {
            GroupTypes.GroupsCreated.Add(newGroup);
            StateHasChanged();
        }
    }

    private void
    HandleGroupEdited
    (
        ClientDto_GroupType? editedGroup
    )
    {
        if (editedGroup != null)
        {
            GroupTypes.GroupsCreated.Find(x => x.GroupId == editedGroup.GroupId).GroupName = editedGroup.GroupName;
            StateHasChanged();
        }
    }

    private void
    HandleGroupDeleted
    (
        Guid groupId
    )
    {
        var deletedGroup = GroupTypes.GroupsCreated.Find(x => x.GroupId == groupId);

        if (deletedGroup is not null)
            GroupTypes.GroupsCreated.Remove(deletedGroup);

        _navigationManager.NavigateTo("/Dashboard");
    }

    private void
    HandleRemovedFromGroup
    (
        Guid groupId
    )
    {
        var deletedGroup = GroupTypes.GroupsJoined.Find(x => x.GroupId == groupId);

        if (deletedGroup is not null)
            GroupTypes.GroupsJoined.Remove(deletedGroup);

        var route = _navigationManager.BaseUri + "Group" + $"/{groupId}";

        if (_navigationManager.Uri == route)
            _navigationManager.NavigateTo("/Dashboard");

        StateHasChanged();
    }

    private void
    HandleGroupInvitationAccepted
    (
        ClientDto_GroupType? group
    )
    {
        if (group != null && GroupTypes is not null && GroupTypes.GroupsJoined is not null)
        {
            GroupTypes.GroupsJoined.Add(group);
            StateHasChanged();
        }
    }

    public void Dispose()
    {
        _appState.OnGroupEdited -= HandleGroupEdited;
        _appState.OnGroupDeleted -= HandleGroupDeleted;
        _appState.OnNewGroupCreated -= HandleNewGroupCreated;
        _appState.OnRemovedFromGroup -= HandleRemovedFromGroup;
        _appState.OnGroupInvitationAccepted -= HandleGroupInvitationAccepted;
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
        var result = await dialog.Result;


    }

    private static DialogOptions
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