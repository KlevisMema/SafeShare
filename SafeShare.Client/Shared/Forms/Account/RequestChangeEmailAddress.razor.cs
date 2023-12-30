using MudBlazor;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientServices.Interfaces;
using SafeShare.ClientDTO.AccountManagment;
using Microsoft.AspNetCore.Components.Forms;

namespace SafeShare.Client.Shared.Forms.Account;

public partial class RequestChangeEmailAddress
{
    [CascadingParameter]
    MudDialogInstance MudDialog { get; set; }

    [Inject]
    private ISnackbar _snackbar { get; set; } = null!;

    [Inject]
    private IClientService_UserManagment _userManagmentService { get; set; } = null!;
    private bool _processing = false;
    private ClientDto_ChangeEmailAddressRequest EmailAddressRequestDto { get; set; } = new();

    private EditForm? changeEmailForm;

    private void
    Cancel()
    {
        MudDialog.Cancel();
    }

    private async Task
    ValidateForm()
    {
        var validationsPassed = changeEmailForm!.EditContext!.Validate()!;

        if (!validationsPassed)
            ShowValidationsMessages(changeEmailForm.EditContext.GetValidationMessages());
        else
            await SubmitUserDataUpdateForm();
    }

    private async Task
    SubmitUserDataUpdateForm()
    {
        _processing = true;
        await Task.Delay(1000);

        var updateDataResult = await _userManagmentService.RequestChangeEmail(EmailAddressRequestDto);

        if (!updateDataResult.Succsess)
        {
            _snackbar.Add(updateDataResult.Message, Severity.Warning, config => { config.CloseAfterNavigation = true; });

            if (updateDataResult.Errors is not null)
                ShowValidationsMessages(updateDataResult.Errors);
        }
        else
        {
            _snackbar.Add(updateDataResult.Message, Severity.Success, config => { config.CloseAfterNavigation = true; });
        }

        EmailAddressRequestDto = new();
        _processing = false;
    }

    private void
    ShowValidationsMessages
    (
        IEnumerable<string> validationMessages
    )
    {
        foreach (var validationMessage in validationMessages)
        {
            _snackbar.Add(validationMessage, Severity.Warning, config => { config.CloseAfterNavigation = true; config.VisibleStateDuration = 3000; });
        }
    }
}