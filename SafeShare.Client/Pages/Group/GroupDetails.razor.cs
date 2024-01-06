using MudBlazor;
using Microsoft.JSInterop;
using SafeShare.Client.Helpers;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientDTO.GroupManagment;
using SafeShare.Client.Shared.Forms.Group;
using Microsoft.AspNetCore.Components.Forms;
using SafeShare.ClientServices.GroupManagment;
using System.Text;
using System.Web;

namespace SafeShare.Client.Pages.Group;

public partial class GroupDetails
{
    [Parameter] public Guid groupId { get; set; }
    [Inject] IDialogService DialogService { get; set; } = null!;
    [Inject] private AppState _appState { get; set; } = null!;
    [Inject] IClientService_GroupManagment _groupManagmentService { get; set; } = null!;
    [Inject] private ISnackbar _snackbar { get; set; } = null!;
    [Inject] IJSRuntime _jSRuntime { get; set; } = null!;
    [Inject] NavigationManager _navManager { get; set; } = null!;
    private ClientDto_GroupDetails? GroupDetailsDto { get; set; }
    private List<ClientDto_UsersGroupDetails>? SelectedUsers { get; set; } = [];
    private ClientDto_EditGroup? EditGroup { get; set; } = new();
    private EditForm? EditGroupForm;
    private MudMessageBox? mbox { get; set; }
    private MudMessageBox? mbox2 { get; set; }
    public bool ReadOnly { get; set; } = true;
    private int screenWidth;
    private bool _processing = false;
    private bool _processingDeleteGroup = false;
    private bool _checked { get; set; }
    private Position TabPosition { get; set; } = Position.Left;
    private string selectedUserBtnText { get; set; } = "Select";
    private string iconSelected = Icons.Material.Filled.SelectAll;

    protected override Task OnInitializedAsync()
    {
        GroupDetailsDto = new();

        return base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        var getGroupDetails = await _groupManagmentService.GetGroupDetails(groupId);

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

        EditGroup.GroupName = HttpUtility.HtmlEncode(EditGroup.GroupName);
        EditGroup.GroupDescription = HttpUtility.HtmlEncode(EditGroup.GroupDescription);

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

    private async void
    RemoveUsers()
    {
        _processingDeleteGroup = true;

        var deleteResult = await _groupManagmentService.DeleteUsersFromGroup(groupId, SelectedUsers);

        foreach (var item in SelectedUsers)
        {
            GroupDetailsDto.UsersGroups.Remove(GroupDetailsDto.UsersGroups.Find(x => x.UserName == item.UserName));
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

        StateHasChanged();
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

        var deleteResult = await _groupManagmentService.DeleteGroup(groupId);

        _snackbar.Add(deleteResult.Message, deleteResult.StatusCode == System.Net.HttpStatusCode.OK ? Severity.Success : Severity.Warning, config => { config.CloseAfterNavigation = true; config.VisibleStateDuration = 3000; });

        if (deleteResult.Succsess)
        {
            _appState.GroupDeleted(groupId);
        }

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
}