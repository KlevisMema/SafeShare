using Microsoft.AspNetCore.Components;
using MudBlazor;
using SafeShare.ClientDTO.Authentication;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace SafeShare.Client.Shared.Forms;

public partial class LoginForm
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }

    void Submit() => MudDialog.Close(DialogResult.Ok(true));
    void Cancel() => MudDialog.Cancel();

    bool success;
    string[] errors = { };
    MudTextField<string> pwField1;
    public MudForm form;

    public ClientDto_Login clientDto_Login { get; set; } = new ClientDto_Login();
}