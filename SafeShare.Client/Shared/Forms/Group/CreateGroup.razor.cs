using MudBlazor;
using SafeShare.Client.Helpers;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientDTO.GroupManagment;
using Microsoft.AspNetCore.Components.Web;
using SafeShare.ClientServices.Interfaces;
using SafeShare.ClientDTO.AccountManagment;
using Microsoft.AspNetCore.Components.Forms;
using SafeShare.ClientServices.GroupManagment;

namespace SafeShare.Client.Shared.Forms.Group;

public partial class CreateGroup
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    [Inject] private ISnackbar _snackbar { get; set; } = null!;
    [Inject] private AppState _appState { get; set; } = null!;
    [Inject] private IClientService_GroupManagment _userGroupeManagment { get; set; } = null!;

    private ClientDto_CreateGroup createGroup { get; set; } = new();

    InputType GroupNameInput = InputType.Text;

    private bool _processing = false;
    private EditForm? CreateGroupForm;

    bool isShow;

    private async Task
    ValidateForm()
    {
        var validationsPassed = CreateGroupForm!.EditContext!.Validate()!;

        if (!validationsPassed)
            ShowValidationsMessages(CreateGroupForm.EditContext.GetValidationMessages());
        else
            await SubmitCreateGroupForm();
    }

    private async Task
    SubmitCreateGroupForm()
    {
        _processing = true;
        await Task.Delay(1000);

        var createGroupResult = await _userGroupeManagment.CreateGroup(createGroup);

        if (!createGroupResult.Succsess)
        {
            _snackbar.Add(createGroupResult.Message, Severity.Warning,
                config => { config.CloseAfterNavigation = true; });

            if (createGroupResult.Errors is not null)
                ShowValidationsMessages(createGroupResult.Errors);
        }
        else
        {
            _snackbar.Add(createGroupResult.Message, Severity.Success,
                config => { config.CloseAfterNavigation = true; });

            _appState.NewGroupAdded(createGroupResult.Value);
        }

        createGroup = new();
        _processing = false;
        MudDialog.Close();
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