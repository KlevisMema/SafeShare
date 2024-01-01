using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SafeShare.ClientDTO.GroupManagment;
using SafeShare.ClientServices.GroupManagment;

namespace SafeShare.Client.Pages.Group;

public partial class GroupDetails
{
    [Parameter] public Guid groupId { get; set; }
    [Inject] IClientService_GroupManagment _groupManagmentService { get; set; } = null!;

    [Inject] IJSRuntime _jSRuntime { get; set; } = null!;

    private ClientDto_GroupDetails? GroupDetailsDto { get; set; }

    private int screenWidth;
    private Position TabPosition { get; set; } = Position.Left;


    protected override async Task
    OnInitializedAsync()
    {
        GroupDetailsDto = new();



        var getGroupDetails = await _groupManagmentService.GetGroupDetails(groupId);

        if (getGroupDetails.Succsess && getGroupDetails.Value is not null)
            GroupDetailsDto = getGroupDetails.Value;
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
}