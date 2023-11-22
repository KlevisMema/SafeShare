using MudBlazor;
using Microsoft.JSInterop;
using Blazored.LocalStorage;
using SafeShare.Client.Helpers;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientDTO.Authentication;
using SafeShare.ClientServices.Interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;
using System.Runtime.InteropServices.JavaScript;

namespace SafeShare.Client.Shared.Forms;

public partial class LoginForm
{
    #region Injections
    [Inject] private AppState AppState { get; set; } = null!; 
    [Inject] private ISnackbar _snackbar { get; set; } = null!;
    [Inject] private IJSRuntime _jsInterop { get; set; } = null!;
    
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private IAuthenticationService _authenticationService { get; set; } = null!;
    #endregion

    private EditForm? loginForm;
    private bool _processing = false;
    private ClientDto_Login clientDto_Login { get; set; } = new();

    private const string SnackbarMessage = "Redirecting you in the dashboard page";
    private const string SnackbarMessage1 = "Credentials are being validated";

   

    private async Task
    SubmitLoginForm()
    {
        _processing = true;
        await Task.Delay(2000);

        var loginResult = await _authenticationService.LogInUser(clientDto_Login);

        if (loginResult.Succsess)
        {
            AppState.setClientSecrests(loginResult.Value);
            _snackbar.Add(loginResult.Message, Severity.Success, config => { config.CloseAfterNavigation = true; });
            await Task.Delay(2000);
            _snackbar.Add($"{SnackbarMessage}", Severity.Info, config => { config.CloseAfterNavigation = true; });
            await Task.Delay(500);
            _navigationManager.NavigateTo("/Dashboard");
        }
        else
        {
            _snackbar.Add(loginResult.Message, Severity.Error);
        }

        _processing = false;
    }

    async Task
    ValidateForm()
    {
        var validationPassed = loginForm.EditContext.Validate();

        if (validationPassed)
        {
            _snackbar.Add(SnackbarMessage1, Severity.Info, config => { config.CloseAfterNavigation = true; });
            await SubmitLoginForm();
        }
    }

    private async Task
   ToggleForms()
    {
        await _jsInterop.InvokeVoidAsync("LoginPage");
    }
}