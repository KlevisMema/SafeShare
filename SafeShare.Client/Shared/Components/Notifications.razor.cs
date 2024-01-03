using MudBlazor;
using SafeShare.Client.Helpers;
using SafeShare.Client.Pages.Group;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientDTO.GroupManagment;
using Microsoft.AspNetCore.Components.Forms;
using SafeShare.ClientServices.GroupManagment;

namespace SafeShare.Client.Shared.Components;

public partial class Notifications
{
    [Inject] IClientService_GroupManagment _groupManagmentService { get; set; } = null!;
    [Inject] private ISnackbar _snackbar { get; set; } = null!;
    [Inject] AppState _appState { get; set; } = null!;
    public bool _isOpen = false;
    private bool _processing = false;
    private List<ClientDto_RecivedInvitations> Invitations { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        var getInvitationsResult = await _groupManagmentService.GetGroupsInvitations();


        if (getInvitationsResult != null && getInvitationsResult.Succsess)
        {
            Invitations = getInvitationsResult.Value!.Where(x => x.InvitationStatus == ClientDTO.Enums.InvitationStatus.Pending).ToList();
        }
    }

    public async Task
    ToggleOpenNotifications()
    {
        if (_isOpen)
            _isOpen = false;
        else
            _isOpen = true;

        var getInvitationsResult = await _groupManagmentService.GetGroupsInvitations();

        if (getInvitationsResult != null && getInvitationsResult.Succsess)
        {
            Invitations = getInvitationsResult.Value!.Where(x => x.InvitationStatus == ClientDTO.Enums.InvitationStatus.Pending).ToList();
        }
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
        });



        if (acceptInvitationResult.Succsess)
        {
            var inv = Invitations.Find(x => x.InvitationId == invitation.InvitationId);

            Invitations.Remove(inv);

            _appState.GroupInvitationAccepted(new ClientDto_GroupType
            {
                GroupId = invitation.GroupId,
                GroupName = invitation.GroupName,
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
        _isOpen = false;
        _processing = false;
    }
}