using MudBlazor;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientServices.Interfaces;
using SafeShare.ClientDTO.AccountManagment;

namespace SafeShare.Client.Pages.Account;

public partial class ChangeEmailAddressConfirmation
{
    [Inject] private ISnackbar _snackbar { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private IClientService_UserManagment _userManagmentService { get; set; } = null!;

    private string? token;
    private string? email;
    private bool isVisible = true;
    private bool buttonDisabled = false;
    private string ActivateAccountMessage { get; set; } = string.Empty;

    private ClientDto_ChangeEmailAddressRequestConfirm? ClientDto_ChangeEmailRequestConfirm { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var uri = new Uri(_navigationManager.Uri);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

            token = query["token"];
            email = query["email"];

            ClientDto_ChangeEmailRequestConfirm = new()
            {
                Token = token ?? string.Empty,
                EmailAddress = email ?? string.Empty

            };

            var confirmChangeEmailAddressRequest = await _userManagmentService.ConfirmChangeEmailAddressRequest(ClientDto_ChangeEmailRequestConfirm);

            isVisible = false;

            if (!confirmChangeEmailAddressRequest.Succsess)
            {
                buttonDisabled = false;
                ActivateAccountMessage = "New email was not confirmed, if this message persists please make another request!!";
                _snackbar.Add(confirmChangeEmailAddressRequest.Message, Severity.Warning, config => { config.CloseAfterNavigation = true; });
            }
            else
            {
                buttonDisabled = true;
                ActivateAccountMessage = "Your email was succsessfully changed, you may close this page";
            }

            await InvokeAsync(StateHasChanged);
        }
    }
}