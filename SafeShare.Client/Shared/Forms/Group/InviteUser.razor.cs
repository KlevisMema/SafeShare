using MudBlazor;
using SafeShare.Client.Helpers;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientDTO.GroupManagment;
using Microsoft.AspNetCore.Components.Forms;
using SafeShare.ClientServices.GroupManagment;
using SafeShare.ClientServices.Interfaces;
using SafeShare.ClientDTO.AccountManagment;

namespace SafeShare.Client.Shared.Forms.Group;

public partial class InviteUser
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    [Parameter] public Guid GroupId { get; set; }
    [Inject] private ISnackbar _snackbar { get; set; } = null!;
    [Inject] private IClientService_GroupManagment _groupManagmentService { get; set; } = null!;
    [Inject] private IClientService_UserManagment _userManagmentService { get; set; } = null!;

    private ClientDto_UserSearched? SelectedUser { get; set; }

    private ClientDto_SendInvitationRequest Dto_SendInvitationRequest { get; set; } = new();
    private EditForm? InviteUserToGroupForm;
    private bool _processing = false;
    private string _state;

    private async Task
    ValidateForm()
    {
        var validationsPassed = InviteUserToGroupForm!.EditContext!.Validate()!;

        if (!validationsPassed)
            ShowValidationsMessages(InviteUserToGroupForm.EditContext.GetValidationMessages());
        else
            await SubmitInviteUserToGroupForm();
    }

    private async Task
    SubmitInviteUserToGroupForm()
    {
        _processing = true;
        await Task.Delay(1000);

        Dto_SendInvitationRequest.GroupId = GroupId;
        Dto_SendInvitationRequest.InvitedUserId = Guid.Parse(SelectedUser.UserId);

        var SendInvitationResult = await _groupManagmentService.SendInvitation(Dto_SendInvitationRequest);

        if (!SendInvitationResult.Succsess)
        {
            _snackbar.Add(SendInvitationResult.Message, Severity.Warning,
                config => { config.CloseAfterNavigation = true; });

            if (SendInvitationResult.Errors is not null)
                ShowValidationsMessages(SendInvitationResult.Errors);
        }
        else
        {
            _snackbar.Add(SendInvitationResult.Message, Severity.Success,
                config => { config.CloseAfterNavigation = true; });

        }

        Dto_SendInvitationRequest = new();
        _processing = false;
        MudDialog.Close();
    }

    private async Task<IEnumerable<ClientDto_UserSearched>>
    Search
    (
        string value,
        CancellationToken token
    )
    {
        var result = await _userManagmentService.SearchUserByUserName(value ?? string.Empty, token);

        if (!result.Succsess && result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        {
            _snackbar.Add
            (
                result.Message, 
                result.StatusCode == System.Net.HttpStatusCode.InternalServerError ? Severity.Error : Severity.Warning, 
                config =>
                {
                    config.CloseAfterNavigation = true;
                    config.VisibleStateDuration = 3000;
                }
            );
        }


        return result.Value ?? Enumerable.Empty<ClientDto_UserSearched>();
    }

    private void
    ShowValidationsMessages
    (
        IEnumerable<string> validationMessages
    )
    {
        foreach (var validationMessage in validationMessages)
        {
            _snackbar.Add(validationMessage, Severity.Warning, config =>
            {
                config.CloseAfterNavigation = true;
                config.VisibleStateDuration = 3000;
            });
        }
    }
}