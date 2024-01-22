using MudBlazor;
using SafeShare.Client.Internal;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientDTO.GroupManagment;
using SafeShare.ClientServices.GroupManagment;

namespace SafeShare.Client.Pages.Group;

public partial class RecivedInvitation
{
    [Inject] IClientService_GroupManagment _groupManagmentService { get; set; } = null!;
    [Inject] AppState _appState { get; set; } = null!;
    [Inject] public ISnackbar _snackbar { get; set; } = null!;
    private List<ClientDto_RecivedInvitations> RecivedInvitations { get; set; } = [];

    private bool _processingRejectInvitation = false;
    private bool _processingAcceptInvitation = false;

    protected override void OnInitialized()
    {
        _appState.OnGroupInvitationAccepted += HandleGroupInvitationAccepted;

        base.OnInitialized();
    }

    public void
    Dispose()
    {
        _appState.OnGroupInvitationAccepted -= HandleGroupInvitationAccepted;
    }

    protected override async Task OnInitializedAsync()
    {
        var getSentInvitations = await _groupManagmentService.GetGroupsInvitations();

        if (getSentInvitations != null && getSentInvitations.Succsess && getSentInvitations.Value is not null)
            RecivedInvitations = getSentInvitations.Value;
    }

    private void
    HandleGroupInvitationAccepted
    (
        ClientDto_GroupType? group
    )
    {
        if (group is not null)
        {
            var findInvitation = RecivedInvitations.Find(inv => inv.GroupId == group.GroupId);

            if (findInvitation is not null)
                findInvitation.InvitationStatus = ClientDTO.Enums.InvitationStatus.Accepted;

            StateHasChanged();
        }
    }

    private async Task
    RejectInvitation
    (
        ClientDto_RecivedInvitations recivedInvitation
    )
    {
        _processingRejectInvitation = true;

        var rejectInvitationResult = await _groupManagmentService.RejectInvitation(new ClientDto_InvitationRequestActions
        {
            GroupId = recivedInvitation.GroupId,
            InvitingUserId = recivedInvitation.InvitingUserId,
            InvitationId = recivedInvitation.InvitationId,
        });

        if (rejectInvitationResult.Succsess)
            RecivedInvitations.Remove(recivedInvitation);

        _processingRejectInvitation = false;
    }

    private async Task
    AcceptInvitation
    (
        ClientDto_RecivedInvitations recivedInvitation
    )
    {
        _processingAcceptInvitation = true;

        var acceptInvitationResult = await _groupManagmentService.AcceptInvitation(new ClientDto_InvitationRequestActions
        {
            GroupId = recivedInvitation.GroupId,
            InvitingUserId = recivedInvitation.InvitingUserId,
            InvitationId = recivedInvitation.InvitationId,
        });

        if (acceptInvitationResult.Succsess)
        {
            RecivedInvitations.Find(x => x.InvitationId == recivedInvitation.InvitationId).InvitationStatus = ClientDTO.Enums.InvitationStatus.Accepted;

            _appState.GroupInvitationAccepted(new ClientDto_GroupType
            {
                GroupId = recivedInvitation.GroupId,
                GroupName = recivedInvitation.GroupName,
            });
        }

        switch (acceptInvitationResult.StatusCode)
        {
            case System.Net.HttpStatusCode.OK:
                _snackbar.Add(acceptInvitationResult.Message, Severity.Success, config => { config.CloseAfterNavigation = true; config.VisibleStateDuration = 3000; });
                break;
            case System.Net.HttpStatusCode.BadRequest:
                _snackbar.Add(acceptInvitationResult.Message, Severity.Warning, config => { config.CloseAfterNavigation = true; config.VisibleStateDuration = 3000; });
                break;
            case System.Net.HttpStatusCode.InternalServerError:
                _snackbar.Add(acceptInvitationResult.Message, Severity.Error, config => { config.CloseAfterNavigation = true; config.VisibleStateDuration = 3000; });
                break;
            default:
                break;
        }

        _processingAcceptInvitation = false;
    }
}