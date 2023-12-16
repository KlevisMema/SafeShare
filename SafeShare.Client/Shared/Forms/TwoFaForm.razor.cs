using MudBlazor;
using Blazored.LocalStorage;
using SafeShare.Client.Helpers;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientDTO.Authentication;
using SafeShare.ClientServices.Interfaces;
using Microsoft.AspNetCore.Components.Forms;

namespace SafeShare.Client.Shared.Forms;

public partial class TwoFaForm
{
    [Parameter] public string? userId { get; set; }
    [Inject] private AppState AppState { get; set; } = null!;
    [Inject] private ISnackbar _snackbar { get; set; } = null!;
    [Inject] private ILocalStorageService _localStorage { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private IAuthenticationService _authenticationService { get; set; } = null!;

    private EditForm? TwoFAForm;
    private bool _processing = false;
    private ClientDto_2FA TwoFA { get; set; } = new();

    private const string SnackbarMessage = "Redirecting you in the dashboard page";

    private async Task
    ConfirmLogin2FA()
    {
        var validationPassed = TwoFAForm!.EditContext!.Validate();

        if (!validationPassed)
        {
            ShowValidationsMessages(TwoFAForm.EditContext.GetValidationMessages());
            return;
        }

        await ConfirmLogin2FASubmit();
    }

    private async Task
    ConfirmLogin2FASubmit()
    {
        _snackbar.Add("Validating the OTP", Severity.Info, config => { config.CloseAfterNavigation = true; });

        _processing = true;
        await Task.Delay(1000);

        try
        {
            TwoFA.UserId = Guid.Parse(userId);
        }
        catch (Exception)
        {
            _snackbar.Add("An unexpected error happened, try again", Severity.Error, config => { config.CloseAfterNavigation = true; });
            TwoFA = new();
            _processing = false;
            return;
        }

        var twoFaResult = await _authenticationService.ConfirmLogin2FA(TwoFA);

        if (!twoFaResult.Succsess)
        {
            if (String.IsNullOrEmpty(twoFaResult.Message))
                _snackbar.Add("An unexpected error happened", Severity.Error, config => { config.CloseAfterNavigation = true; });
            else
                _snackbar.Add(twoFaResult.Message, Severity.Warning, config => { config.CloseAfterNavigation = true; });
            //ShowValidationsMessages(registerResult.Errors!);
        }
        else
        {
            AppState.SetClientSecrests(twoFaResult.Value);
            _snackbar.Add(twoFaResult.Message, Severity.Success, config => { config.CloseAfterNavigation = true; });
            await Task.Delay(2000);
            _snackbar.Add($"{SnackbarMessage}", Severity.Info, config => { config.CloseAfterNavigation = true; });
            await Task.Delay(500);
            await _localStorage.SetItemAsStringAsync("UserData", twoFaResult.Value!.UserId);
            _navigationManager.NavigateTo("/Dashboard");
        }

        TwoFA = new();
        _processing = false;
    }

    private void
    ShowValidationsMessages
    (
        IEnumerable<string> validationMessages
    )
    {
        foreach (var validationMessage in validationMessages)
            _snackbar.Add(validationMessage, Severity.Warning, config => { config.CloseAfterNavigation = true; });
    }
}