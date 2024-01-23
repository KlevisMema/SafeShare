using MudBlazor;
using System.Net;
using Newtonsoft.Json.Linq;
using Blazored.LocalStorage;
using SafeShare.Client.Internal;
using SafeShare.Client.Pages.Group;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientDTO.Notification;
using SafeShare.ClientDTO.GroupManagment;
using SafeShare.ClientServices.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components.Forms;
using SafeShare.ClientServices.GroupManagment;

namespace SafeShare.Client.Shared.Components;

public partial class Notifications : IAsyncDisposable, IDisposable
{
    [Inject] AppState _appState { get; set; } = null!;
    [Inject] private ISnackbar _snackbar { get; set; } = null!;
    [Inject] private SignalRService _signalR { get; set; } = null!;
    [Inject] private ILocalStorageService _localStorage { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] IAuthenticationService _authenticationService { get; set; } = null!;
    [Inject] IClientService_GroupManagment _groupManagmentService { get; set; } = null!;

    public bool _isOpen = false;
    private bool _processing = false;
    private bool badgeVisible = false;
    private bool _processingNotificationDelete = false;

    private List<ClientDto_RecivedInvitations> Invitations { get; set; } = [];
    private List<ClientDto_Notification> NotificationsList { get; set; } = [];

    private string BtnText { get; set; } = "Accept";

    protected override async Task OnInitializedAsync()
    {
        _appState.OnLogOut += HandleLogOut;

        HandleSignalR();

        var getInvitationsResult = await _groupManagmentService.GetGroupsInvitations();

        if (getInvitationsResult != null && getInvitationsResult.Succsess)
            Invitations = getInvitationsResult.Value!.Where(x => x.InvitationStatus == ClientDTO.Enums.InvitationStatus.Pending).ToList();
    }

    private void
    HandleSignalR()
    {
        if (_signalR.HubConnection is not null)
        {
            _signalR.HubConnection.On<Guid, string>("GroupDeleted", HandleGroupDeleted);
            _signalR.HubConnection.On("ReceiveGroupInvitation", HandleGroupInvitationIndication);
            _signalR.HubConnection.On<string, Guid>("RemovedFromTheGroup", HandleRemovedFromTheGroup);
            _signalR.HubConnection.On<string, string>("AcceptedInvitation", HandleAcceptedInvitation);
        }
    }

    private async Task
    HandleGroupInvitationIndication()
    {
        _snackbar.Add("Invitation received!", Severity.Info, config =>
        {
            config.CloseAfterNavigation = true;
            config.VisibleStateDuration = 1000;
            config.DuplicatesBehavior = SnackbarDuplicatesBehavior.Prevent;
            config.InfoIcon = Icons.Material.Outlined.Notifications;
        });

        badgeVisible = true;

        await InvokeAsync(StateHasChanged);
    }

    private void
    HandleRemovedFromTheGroup
    (
        string groupName,
        Guid groupId
    )
    {
        _snackbar.Add($"You have been removed a group!", Severity.Info, config =>
        {
            config.CloseAfterNavigation = true;
            config.VisibleStateDuration = 1000;
            config.DuplicatesBehavior = SnackbarDuplicatesBehavior.Prevent;
            config.InfoIcon = Icons.Material.Outlined.Notifications;
        });

        badgeVisible = true;

        NotificationsList.Add(new ClientDto_Notification
        {
            NotificationId = Guid.NewGuid(),
            NotificationMessage = $"You have been removed from {groupName} group",
        });

        _appState.RemovedFromGroup(groupId);

        StateHasChanged();
    }

    private void
    HandleGroupDeleted
    (
        Guid groupId,
        string groupName
    )
    {
        _snackbar.Add($"A group was deleted!", Severity.Info, config =>
        {
            config.CloseAfterNavigation = true;
            config.VisibleStateDuration = 1000;
            config.DuplicatesBehavior = SnackbarDuplicatesBehavior.Prevent;
            config.InfoIcon = Icons.Material.Outlined.Notifications;
        });

        badgeVisible = true;

        NotificationsList.Add(new ClientDto_Notification
        {
            NotificationId = Guid.NewGuid(),
            NotificationMessage = $"Group {groupName} was just deleted!",
        });

        _appState.RemovedFromGroup(groupId);

        StateHasChanged();
    }

    private void
    HandleAcceptedInvitation
    (
        string groupName,
        string userWhoAcceptedInvitation
    )
    {
        _snackbar.Add($"Invitation accepted!", Severity.Info, config =>
        {
            config.CloseAfterNavigation = true;
            config.VisibleStateDuration = 1000;
            config.DuplicatesBehavior = SnackbarDuplicatesBehavior.Prevent;
            config.InfoIcon = Icons.Material.Outlined.Notifications;
        });

        badgeVisible = true;

        NotificationsList.Add(new ClientDto_Notification
        {
            NotificationId = Guid.NewGuid(),
            NotificationMessage = $"{userWhoAcceptedInvitation} accepted your invitation to join to {groupName} group",
        });

        StateHasChanged();
    }

    private async void
    HandleLogOut()
    {
        await _signalR.DisconnectAsync();
    }

    public async Task
    ToggleOpenNotifications()
    {
        if (_isOpen)
            _isOpen = false;
        else
        {
            _isOpen = true;

            var getInvitationsResult = await _groupManagmentService.GetGroupsInvitations();

            if (getInvitationsResult != null && getInvitationsResult.Succsess)
                Invitations = getInvitationsResult.Value!.Where(x => x.InvitationStatus == ClientDTO.Enums.InvitationStatus.Pending).ToList();
        }

        badgeVisible = false;
    }

    private async Task
    AcceptInvitation
    (
        ClientDto_RecivedInvitations invitation
    )
    {
        _processing = true;

        var acceptInvitationResult = await _groupManagmentService.AcceptInvitation(new ClientDto_InvitationRequestActions
        {
            GroupId = invitation.GroupId,
            InvitingUserId = invitation.InvitingUserId,
            InvitationId = invitation.InvitationId,
            GroupName = invitation.GroupName,
            UserWhoAcceptedTheInvitation = await _localStorage.GetItemAsStringAsync("FullName") ?? string.Empty
        });

        ShowAcceptInvitationResult(acceptInvitationResult.StatusCode, acceptInvitationResult.Message ?? string.Empty);

        if (acceptInvitationResult.Succsess)
        {
            var inv = Invitations.Find(x => x.InvitationId == invitation.InvitationId);

            if (inv is not null)
                Invitations.Remove(inv);
            else
            {
                _processing = false;
                BtnText = "Accepted";
            }

            _appState.GroupInvitationAccepted(new ClientDto_GroupType
            {
                GroupId = invitation.GroupId,
                GroupName = invitation.GroupName,
            });

            var getGroupDetails = await _groupManagmentService.GetGroupDetails(invitation.GroupId);

            if (getGroupDetails.Succsess && getGroupDetails.Value is not null)
                _appState.GroupDetails(getGroupDetails.Value);
        }

        _isOpen = false;
        _processing = false;
    }

    private void
    ShowAcceptInvitationResult
    (
        HttpStatusCode httpStatusCode,
        string message
    )
    {
        switch (httpStatusCode)
        {
            case HttpStatusCode.OK:
                _snackbar.Add(message, Severity.Success, config => { config.CloseAfterNavigation = true; config.VisibleStateDuration = 3000; });
                break;
            case HttpStatusCode.BadRequest:
                _snackbar.Add(message, Severity.Warning, config => { config.CloseAfterNavigation = true; config.VisibleStateDuration = 3000; });
                break;
            case HttpStatusCode.InternalServerError:
                _snackbar.Add(message, Severity.Error, config => { config.CloseAfterNavigation = true; config.VisibleStateDuration = 3000; });
                break;
            default:
                break;
        }
    }

    private void
    DeleteNotification
    (
        Guid notificationId
    )
    {
        _processingNotificationDelete = true;

        Task.Delay(1000);

        var notificationToDelete = NotificationsList.FirstOrDefault(x => x.NotificationId == notificationId);

        if (notificationToDelete != null)
            NotificationsList.Remove(notificationToDelete);

        _processingNotificationDelete = false;

        StateHasChanged();
    }

    public void
    Dispose()
    {
        _appState.OnLogOut -= HandleLogOut;
    }

    public async ValueTask
    DisposeAsync()
    {
        await _signalR.DisposeAsync();
    }
}