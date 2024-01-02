using MudBlazor;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientDTO.GroupManagment;
using SafeShare.ClientServices.GroupManagment;
using Microsoft.AspNetCore.Components.Forms;

namespace SafeShare.Client.Pages.Group;

public partial class GroupDetails
{
    [Parameter] public Guid groupId { get; set; }
    [Inject] IClientService_GroupManagment _groupManagmentService { get; set; } = null!;

    [Inject] IJSRuntime _jSRuntime { get; set; } = null!;

    private ClientDto_GroupDetails? GroupDetailsDto { get; set; }
    private ClientDto_EditGroup? EditGroup { get; set; } = new();
    private EditForm? EditGroupForm;
    public bool ReadOnly { get; set; } = true;

    private int screenWidth;
    private Position TabPosition { get; set; } = Position.Left;

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
            GroupDetailsDto = new();
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

    private void OnValidSubmit(EditContext context)
    {
        StateHasChanged();
    }

}