using MudBlazor;
using SafeShare.Client.Internal;
using SafeShare.ClientDTO.Expense;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientServices.Interfaces;
using Microsoft.AspNetCore.Components.Forms;

namespace SafeShare.Client.Shared.Forms.Expense;

public partial class EditExpense
{
    [Parameter] public Guid ExpenseId { get; set; }
    [Parameter] public AppState? _appState { get; set; }
    [Parameter] public ClientDto_Expense? Expense { get; set; }
    [Parameter] public ISnackbar _snackbar { get; set; } = null!;
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    [Parameter] public ClientDto_ExpenseCreate? EditExpenseDto { get; set; }
    [Parameter] public IClientService_ExpenseManagment _expenseManagmentService { get; set; } = null!;

    private EditForm? EditExpenseForm;
    private bool _processing = false;

    private async Task
    SubmitEditExpenseForm()
    {
        _processing = true;
        await Task.Delay(1000);

        EditExpenseDto.Date = DateTime.UtcNow.ToString();

        var editExpenseResult = await _expenseManagmentService.EditExpense(ExpenseId, EditExpenseDto);

        if (editExpenseResult.Succsess && editExpenseResult.Value is not null)
        {
            Expense.Title = editExpenseResult.Value.Title;
            Expense.Amount = editExpenseResult.Value.Amount;
            Expense.Description = editExpenseResult.Value.Description;
            Expense.Date = editExpenseResult.Value.Date;

            _appState.ExpenseEditted(Expense);
        }

        _snackbar.Add(editExpenseResult.Message, editExpenseResult.StatusCode == System.Net.HttpStatusCode.OK ? Severity.Success : Severity.Warning, config => { config.CloseAfterNavigation = true; config.VisibleStateDuration = 3000; });
        _processing = false;
        MudDialog.Close();
        await InvokeAsync(StateHasChanged);
    }


    private async Task
    ValidateForm()
    {
        var validationsPassed = EditExpenseForm!.EditContext!.Validate()!;

        if (!validationsPassed)
            ShowValidationsMessages(EditExpenseForm.EditContext.GetValidationMessages());
        else
            await SubmitEditExpenseForm();
    }

    private void
    ShowValidationsMessages
    (
        IEnumerable<string> validationMessages
    )
    {
        foreach (var validationMessage in validationMessages)
            _snackbar.Add(validationMessage, Severity.Warning, config => { config.CloseAfterNavigation = true; config.VisibleStateDuration = 3000; });
    }
}