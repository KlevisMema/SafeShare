using MudBlazor;
using Microsoft.JSInterop;
using Blazored.LocalStorage;
using SafeShare.Client.Internal;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientDTO.Authentication;
using SafeShare.Client.Shared.Forms.Group;
using SafeShare.ClientServices.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using System.Security.Cryptography.X509Certificates;

namespace SafeShare.Client.Shared.Forms.Auth;

public partial class KeysGeneration
{
    [Inject] private AppState AppState { get; set; } = null!;
    [Inject] private ISnackbar _snackbar { get; set; } = null!;
    [Inject] private IJSRuntime _jsInterop { get; set; } = null!;
    [Inject] private ILocalStorageService _localStorage { get; set; } = null!;
    [Inject] private IAuthenticationService _authenticationService { get; set; } = null!;

    private bool _processing = false;
    private EditForm? CreateSecretPassPhraseForm;
    [CascadingParameter] MudDialogInstance? MudDialog { get; set; }
    private ClientDto_PassPhrase dto_PassPhrase { get; set; } = new();

    private const string GeneralFailKeys = "Something went wrong, please try again!";
    private const string EndOfOperation = "Operation finished, we invite you to log in again!";
    private const string KeysGenerated = "Keys successfully generated and stored in your browser!";

    private async Task
    ValidateFormOfUserGivingPassPhraseFirsTime()
    {
        var validationsPassed = CreateSecretPassPhraseForm!.EditContext!.Validate()!;

        if (!validationsPassed)
            ShowValidationsMessages(CreateSecretPassPhraseForm.EditContext.GetValidationMessages());
        else
            await SubmitCreateKeysForm();
    }

    private async Task
    ValidateFormOfUserHavingKeyInServerNotInBrowser()
    {
        var validationsPassed = CreateSecretPassPhraseForm!.EditContext!.Validate()!;

        if (!validationsPassed)
            ShowValidationsMessages(CreateSecretPassPhraseForm.EditContext.GetValidationMessages());
        else
            await ValidateKey();
    }

    private async Task
    ValidateKey()
    {

    }

    private void
    ShowValidationsMessages
    (
        IEnumerable<string> validationMessages
    )
    {
        foreach (var validationMessage in validationMessages)
        {
            _snackbar.Add(validationMessage, Severity.Error, config =>
            {
                config.CloseAfterNavigation = true;
                config.VisibleStateDuration = 3000;
            });
        }
    }

    private async Task
    SubmitCreateKeysForm()
    {
        _processing = true;

        await GenerateKeys();

        _processing = false;
        MudDialog.Close();
    }

    private async Task GenerateKeys()
    {
        string? userId = await _localStorage.GetItemAsStringAsync("Id");

        bool successGenerateKeys = await _jsInterop.InvokeAsync<bool>("generateKeys", dto_PassPhrase.SecretPassPhrase, userId);

        if (!successGenerateKeys)
        {
            _snackbar.Add(GeneralFailKeys, Severity.Error, options =>
            {
                options.CloseAfterNavigation = true;
            });
            _processing = false;
        }

        var result = await StorePublicKeyInServer(userId);

        if (!result)
            return;

        _snackbar.Add(KeysGenerated, Severity.Success, options =>
        {
            options.CloseAfterNavigation = true;
        });
    }

    private async Task<bool> StorePublicKeyInServer
    (
        string userId
    )
    {
        string? publicKey = await _jsInterop.InvokeAsync<string>("getPublicKey", userId);

        var savePublicKey = await _authenticationService.SaveUserPublicKey(userId, publicKey);

        if (!savePublicKey.Succsess)
        {
            _snackbar.Add(savePublicKey.Message, Severity.Error, options =>
            {
                options.CloseAfterNavigation = true;
            });
            _processing = false;
            return false;
        }
        return true;
    }
}