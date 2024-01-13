using MudBlazor;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientServices.Interfaces;
using SafeShare.ClientDTO.AccountManagment;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace SafeShare.Client.Shared.Forms.Account;

public enum FieldVariant
{
    OldPassword = 1,
    NewPassword = 2,
    ConfirmNewPassword = 3
}

public partial class ChangePassword
{
    [CascadingParameter]
    MudDialogInstance MudDialog { get; set; }
    [Inject] private ISnackbar _snackbar { get; set; } = null!;
    [Inject] private IClientService_UserManagment _userManagmentService { get; set; } = null!;

    private ClientDto_UserChangePassword UserChangePassword { get; set; } = new();

    InputType PasswordInput = InputType.Password;
    string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

    private bool _processing = false;
    private EditForm? ChangePasswordForm;

    bool isShow;

    private async Task
    ValidateForm()
    {
        var validationsPassed = ChangePasswordForm!.EditContext!.Validate()!;

        if (!validationsPassed)
            ShowValidationsMessages(ChangePasswordForm.EditContext.GetValidationMessages());
        else
            await SubmitUserDataUpdateForm();
    }

    private async Task
    SubmitUserDataUpdateForm()
    {
        _processing = true;
        await Task.Delay(1000);

        var updateDataResult = await _userManagmentService.ChangePassword(UserChangePassword);

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

        UserChangePassword = new();
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

    private void
    DisplayTxtPassword()
    {
        if (isShow)
        {
            isShow = false;
            PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
            PasswordInput = InputType.Password;
        }
        else
        {
            isShow = true;
            PasswordInputIcon = Icons.Material.Filled.Visibility;
            PasswordInput = InputType.Text;
        }
    }
}