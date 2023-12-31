using MudBlazor;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientServices.Interfaces;
using SafeShare.ClientDTO.AccountManagment;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using SafeShare.ClientDTO.GroupManagment;
using SafeShare.ClientServices.GroupManagment;

namespace SafeShare.Client.Shared.Forms.Group;

public partial class CreateGroup
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    [Inject] private ISnackbar _snackbar { get; set; } = null!;
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
            await SubmitUserDataUpdateForm();
    }

    private async Task
        SubmitUserDataUpdateForm()
    {
        _processing = true;
        await Task.Delay(1000);

        var updateDataResult = await _userGroupeManagment.CreateGroup(createGroup);

        if (!updateDataResult.Succsess)
        {
            _snackbar.Add(updateDataResult.Message, Severity.Warning,
                config => { config.CloseAfterNavigation = true; });

            if (updateDataResult.Errors is not null)
                ShowValidationsMessages(updateDataResult.Errors);
        }
        else
        {
            _snackbar.Add(updateDataResult.Message, Severity.Success,
                config => { config.CloseAfterNavigation = true; });
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