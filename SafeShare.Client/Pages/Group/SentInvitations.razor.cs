using Microsoft.AspNetCore.Components;
using MudBlazor;
using SafeShare.Client.Helpers;
using SafeShare.ClientDTO.GroupManagment;
using SafeShare.ClientServices.GroupManagment;

namespace SafeShare.Client.Pages.Group;

public partial class SentInvitations
{
    [Inject] IClientService_GroupManagment _groupManagmentService { get; set; } = null!;
    [Inject] public ISnackbar _snackbar { get; set; } = null!;
    private List<ClientDto_SentInvitations> SentInvitationsList { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {

        var getSentInvitations = await _groupManagmentService.GetSentGroupInvitations();

        if (getSentInvitations != null && getSentInvitations.Succsess && getSentInvitations.Value is not null)
            SentInvitationsList = getSentInvitations.Value;
    }

    private async void
    DeleteInvitation
    (
        ClientDto_SentInvitations SentInvitation
    )
    {
        var deleteInvitationResult = await _groupManagmentService.DeleteInvitation(new ClientDto_InvitationRequestActions
        {
            GroupId = SentInvitation.GroupId,
            InvitedUserId = SentInvitation.InvitedUserId,
            InvitationId = SentInvitation.InvitationId,
        });

        if (deleteInvitationResult.Succsess)
            SentInvitationsList.Find(x => x.InvitationId == SentInvitation.InvitationId).InvitationStatus = ClientDTO.Enums.InvitationStatus.Rejected;

        switch (deleteInvitationResult.StatusCode)
        {
            case System.Net.HttpStatusCode.OK:
                _snackbar.Add(deleteInvitationResult.Message, Severity.Success, config => { config.CloseAfterNavigation = true; config.VisibleStateDuration = 3000; });
                break;
            case System.Net.HttpStatusCode.BadRequest:
                _snackbar.Add(deleteInvitationResult.Message, Severity.Warning, config => { config.CloseAfterNavigation = true; config.VisibleStateDuration = 3000; });
                break;
            case System.Net.HttpStatusCode.InternalServerError:
                _snackbar.Add(deleteInvitationResult.Message, Severity.Error, config => { config.CloseAfterNavigation = true; config.VisibleStateDuration = 3000; });
                break;
            default:
                break;
        }

        StateHasChanged();
    }
}