using MudBlazor;
using System.Web;
using System.Text;
using Microsoft.JSInterop;
using SafeShare.Client.Internal;
using SafeShare.ClientDTO.Expense;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientDTO.GroupManagment;
using SafeShare.Client.Shared.Forms.Group;
using SafeShare.ClientServices.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using SafeShare.Client.Shared.Forms.Expense;
using SafeShare.ClientServices.GroupManagment;
using Blazored.LocalStorage;

namespace SafeShare.Client.Pages.Group;

public partial class GroupDetails
{
    [Parameter] public Guid groupId { get; set; }
    [Inject] IJSRuntime _jSRuntime { get; set; } = null!;
    [Inject] private AppState _appState { get; set; } = null!;
    [Inject] private ISnackbar _snackbar { get; set; } = null!;
    [Inject] IDialogService DialogService { get; set; } = null!;
    [Inject] NavigationManager _navManager { get; set; } = null!;
    [Inject] private ILocalStorageService _localStorage { get; set; } = null!;
    [Inject] IClientService_GroupManagment _groupManagmentService { get; set; } = null!;
    [Inject] IClientService_ExpenseManagment _expenseManagmentService { get; set; } = null!;


    private EditForm? EditGroupForm;
    private EditForm? CreateExpenseForm;
    MudDataGrid<ClientDto_Expense>? dataGrid;
    private MudMessageBox? mbox { get; set; }
    private MudMessageBox? mbox2 { get; set; }
    private MudMessageBox? mbox3 { get; set; }
    private List<ClientDto_Expense>? Expenses { get; set; } = [];
    private ClientDto_EditGroup? EditGroup { get; set; } = new();
    private ClientDto_GroupDetails GroupDetailsDto { get; set; } = null!;
    private ClientDto_Expense SelectedExpense { get; set; } = new();
    private ClientDto_Expense? SelectedExpenseForDeletion { get; set; }
    private ClientDto_ExpenseCreate CreateExpenseModel { get; set; } = new();
    private List<ClientDto_UsersGroupDetails> SelectedUsers { get; set; } = [];
    private ClientDto_ExpenseCreate? EditExpense { get; set; }

    private int screenWidth;
    private string _searchString;
    private bool _processing = false;
    private bool _checked { get; set; }
    private bool _refreshData = false;
    public bool ReadOnly { get; set; } = false;
    private bool _processingDeleteGroup = false;
    private static bool _customizeGroupBy = true;
    private bool _processingDeleteExpense = false;
    private bool _processingCreateExpense = false;
    private Position TabPosition { get; set; } = Position.Left;
    private string selectedUserBtnText { get; set; } = "Select";
    private string iconSelected = Icons.Material.Filled.SelectAll;

    protected override Task
    OnInitializedAsync()
    {
        _appState.OnExpenseEditted += ExpenseEdited;
        GroupDetailsDto = new();

        return base.OnInitializedAsync();
    }

    protected override async Task
    OnParametersSetAsync()
    {
        var getGroupDetails = await _groupManagmentService.GetGroupDetails(groupId);
        var getExpenseDetails = await _expenseManagmentService.GetAllExpensesOfGroup(groupId);

        if (getGroupDetails.Succsess && getGroupDetails.Value is not null)
        {
            GroupDetailsDto = getGroupDetails.Value;
            EditGroup.GroupDescription = getGroupDetails.Value.Description;
            EditGroup.GroupName = getGroupDetails.Value.GroupName;
        }
        else
        {
            _snackbar.Add(getGroupDetails.Message, Severity.Warning, config => { config.CloseAfterNavigation = false; config.VisibleStateDuration = 2000; });
            _navManager.NavigateTo("/Dashboard");
            GroupDetailsDto = new();
        }

        if (getExpenseDetails.Succsess && getExpenseDetails.Value is not null)
            Expenses = getExpenseDetails.Value;
        else
            _snackbar.Add(getExpenseDetails.Message, Severity.Warning, config => { config.CloseAfterNavigation = false; config.VisibleStateDuration = 2000; });
    }

    private void
    ExpenseEdited
    (
        ClientDto_Expense expense
    )
    {
        if (Expenses is not null && Expenses.Count > 0)
        {
            var findExpenseToUpdate = Expenses.Find(x => x.Id == expense.Id);

            if (findExpenseToUpdate != null)
            {
                var index = Expenses.IndexOf(findExpenseToUpdate);
                Expenses[index] = expense;
                StateHasChanged();
            }
        }
    }

    public void
    Dispose()
    {
        _appState.OnExpenseEditted -= ExpenseEdited;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await _jSRuntime.InvokeVoidAsync("registerScreenWidthChanged", DotNetObjectReference.Create(this));
    }

    [JSInvokable]
    public void UpdateScreenWidth(int width)
    {
        screenWidth = width;

        if (screenWidth < 570)
            TabPosition = Position.Top;
        else
            TabPosition = Position.Left;

        StateHasChanged();
    }

    private async Task
    OnValidSubmit
    (
        EditContext context
    )
    {
        _processing = true;
        await Task.Delay(1000);

        var updateResult = await _groupManagmentService.EditGroup(groupId, EditGroup);

        if (updateResult.Succsess)
        {
            GroupDetailsDto.Description = EditGroup.GroupDescription;
            _appState.GroupEdited(updateResult.Value);
        }

        _snackbar.Add(updateResult.Message, updateResult.StatusCode == System.Net.HttpStatusCode.OK ? Severity.Success : Severity.Warning, config => { config.CloseAfterNavigation = true; config.VisibleStateDuration = 3000; });
        GroupDetailsDto.GroupName = EditGroup.GroupName;
        GroupDetailsDto.Description = EditGroup.GroupDescription;
        _processing = false;
    }

    private void
    OnInValidSubmit
    (
        EditContext context
    )
    {
        ShowValidationsMessages(context.GetValidationMessages());
    }

    private async Task
    RemoveUsers()
    {
        _processingDeleteGroup = true;

        var deleteResult = await _groupManagmentService.DeleteUsersFromGroup(groupId, SelectedUsers);

        if (deleteResult.Succsess)
        {
            foreach (var item in SelectedUsers)
            {
                var deletedMember = GroupDetailsDto.UsersGroups.Find(x => x.UserName == item.UserName);

                if (deletedMember != null)
                    GroupDetailsDto.UsersGroups.Remove(deletedMember);

            }

            SelectedUsers.Clear();
        }

        _processingDeleteGroup = false;
        mbox2.Close();

        switch (deleteResult.StatusCode)
        {
            case System.Net.HttpStatusCode.OK:
                _snackbar.Add(deleteResult.Message, Severity.Success, config => { config.CloseAfterNavigation = true; config.VisibleStateDuration = 3000; });
                break;
            case System.Net.HttpStatusCode.BadRequest:
                _snackbar.Add(deleteResult.Message, Severity.Warning, config => { config.CloseAfterNavigation = true; config.VisibleStateDuration = 3000; });
                break;
            case System.Net.HttpStatusCode.InternalServerError:
                _snackbar.Add(deleteResult.Message, Severity.Error, config => { config.CloseAfterNavigation = true; config.VisibleStateDuration = 3000; });
                break;
            default:
                break;
        }

        await InvokeAsync(StateHasChanged);
    }

    private void
    UserChecked
    (
        ClientDto_UsersGroupDetails clientDto
    )
    {
        if (SelectedUsers.Any(x => x.UserName == clientDto.UserName))
        {
            SelectedUsers.Remove(clientDto);
            selectedUserBtnText = "Select";
            iconSelected = Icons.Material.Filled.SelectAll;
        }
        else
        {
            SelectedUsers.Add(clientDto);
            selectedUserBtnText = "Deselect";
            iconSelected = Icons.Material.Filled.Deselect;
        }
    }

    private async Task
    DeleteGroup()
    {
        _processingDeleteGroup = true;

        var deleteResult = await _groupManagmentService.DeleteGroup
        (
            groupId,
            GroupDetailsDto.GroupName,
            GroupDetailsDto.UsersGroups.Where(x => !x.IsAdmin).Select(x => x.UserId).ToList()
        );

        _snackbar.Add(deleteResult.Message, deleteResult.StatusCode == System.Net.HttpStatusCode.OK ? Severity.Success : Severity.Warning, config => { config.CloseAfterNavigation = true; config.VisibleStateDuration = 3000; });

        if (deleteResult.Succsess)
            _appState.GroupDeleted(groupId);

        _processingDeleteGroup = false;
        mbox.Close();
    }

    private async void
    OpenPopUpDeleteConfirmation()
    {
        await mbox.Show();
        StateHasChanged();
    }

    private async void
    RemoveUsersFromGroup()
    {
        if (SelectedUsers.Count == 0)
        {
            _snackbar.Add("You haven't selected any user to remove!", Severity.Warning, config => { config.CloseAfterNavigation = true; config.VisibleStateDuration = 3000; });
            return;
        }

        await mbox2.Show();
        StateHasChanged();
    }

    private async Task
    InviteUserToGroup()
    {
        var parameters = new DialogParameters<InviteUser>
        {
            { x => x.GroupId, groupId }
        };

        var dialog = await DialogService.ShowAsync<InviteUser>("Invite User Dialog", parameters, DialogOptions());
        await dialog.Result;
    }

    private static DialogOptions
    DialogOptions()
    {
        return new()
        {
            ClassBackground = "my-custom-class",
            CloseOnEscapeKey = false,
            DisableBackdropClick = true,
            CloseButton = true,
            Position = DialogPosition.Center
        };
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

    private Func<ClientDto_Expense, bool> _quickFilter => x =>
    {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;

        if (x.Description.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        if (x.Amount.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        if ($"{x.Title}$".Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    };

    private void
    ExpenseSelected
    (
        ClientDto_Expense SelectedExpense
    )
    {
        this.SelectedExpense = SelectedExpense;
    }

    private async Task
    OnValidExpenseSubmit
    (
        EditContext context
    )
    {
        _processingCreateExpense = true;
        await Task.Delay(1000);

        CreateExpenseModel.Date = DateTime.UtcNow.ToString();
        CreateExpenseModel.GroupId = groupId;

        ClientDto_ExpenseCreate encryptedExpense = await _jSRuntime.InvokeAsync<ClientDto_ExpenseCreate>("EncryptExpense", CreateExpenseModel, await _localStorage.GetItemAsStringAsync("Id"));
        
        var createExpenseResult = await _expenseManagmentService.CreateExpense(CreateExpenseModel);


        if (createExpenseResult.Succsess && createExpenseResult.Value is not null)
        {
            Expenses.Add(createExpenseResult.Value);

            if (GroupDetailsDto.ImAdmin)
                _appState.ExpenseCreatedOnGroupsCreated(CreateExpenseModel.Amount);
            else
                _appState.ExpenseCreatedOnGroupsJoined(CreateExpenseModel.Amount);
        }

        _snackbar.Add(createExpenseResult.Message, createExpenseResult.StatusCode == System.Net.HttpStatusCode.OK ? Severity.Success : Severity.Warning, config => { config.CloseAfterNavigation = true; config.VisibleStateDuration = 3000; });
        CreateExpenseModel = new();
        _processingCreateExpense = false;
        await InvokeAsync(StateHasChanged);
    }

    private void
    OnInValidExpenseSubmit
    (
        EditContext context
    )
    {
        ShowValidationsMessages(context.GetValidationMessages());
    }

    private async Task
    OpenFormToEditExpense
    (
        ClientDto_Expense expense
    )
    {
        EditExpense = new()
        {
            Amount = decimal.Parse(expense.Amount),
            Date = expense.Date,
            Description = expense.Description,
            GroupId = expense.GroupId,
            Title = expense.Title,
        };

        var parameters = new DialogParameters<EditExpense>
        {
            { x => x.EditExpenseDto, EditExpense},
            { y => y.ExpenseId, expense.Id },
            { z => z._expenseManagmentService, _expenseManagmentService},
            { d => d._snackbar, _snackbar },
            { c => c.Expense, expense },
            { e => e._appState, _appState },
        };

        await DialogService.ShowAsync<EditExpense>("Edit expense", parameters, DialogOptions());
    }

    private readonly Func<ClientDto_Expense, object> _groupBy = x =>
    {
        if (x.CreatedByMe)
            return "Me";

        return x.CreatorOfExpense;
    };

    private static string
    GroupClassFunc
    (
        GroupDefinition<ClientDto_Expense> item
    )
    {
        return item.Grouping.Key?.ToString() == "Me"
                ? "mud-theme-primary"
                : string.Empty;
    }

    private async Task
    OpenPopUpDeleteExpenseConfirmation
    (
        ClientDto_Expense expense
    )
    {
        SelectedExpenseForDeletion = expense;

        await mbox3.Show();
        StateHasChanged();
    }

    private async Task
    PopUpDeleteExpenseConfirmation()
    {
        _processingDeleteExpense = true;
        if (SelectedExpenseForDeletion is null)
            return;

        ClientDto_ExpenseDelete expenseDelete = new()
        {
            ExpenseAmount = decimal.Parse(SelectedExpenseForDeletion.Amount),
            ExpenseId = SelectedExpenseForDeletion.Id,
            GroupId = SelectedExpenseForDeletion.GroupId
        };

        var deleteExpenseResult = await _expenseManagmentService.DeleteExpense(expenseDelete);

        if (deleteExpenseResult.Succsess)
        {
            var indexForDeletion = Expenses.IndexOf(SelectedExpenseForDeletion);
            Expenses.RemoveAt(indexForDeletion);
        }

        _snackbar.Add(deleteExpenseResult.Message, deleteExpenseResult.StatusCode == System.Net.HttpStatusCode.OK ? Severity.Success : Severity.Warning, config => { config.CloseAfterNavigation = true; config.VisibleStateDuration = 3000; });
        _processingDeleteExpense = false;
        SelectedExpenseForDeletion = null;
        await InvokeAsync(StateHasChanged);
    }

    private void
    CancelingDeletion()
    {
        SelectedExpenseForDeletion = null;
    }

    private async Task
    RefeshData()
    {
        _refreshData = true;
        await OnParametersSetAsync();
        _refreshData = false;
        await InvokeAsync(StateHasChanged);
    }
}