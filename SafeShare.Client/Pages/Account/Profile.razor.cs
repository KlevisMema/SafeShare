using SafeShare.Client.Shared;
using SafeShare.Client.Internal;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientServices.Interfaces;
using SafeShare.ClientDTO.AccountManagment;

namespace SafeShare.Client.Pages.Account;

public partial class Profile
{
    private bool DataRetrieved { get; set; } = false;
    [Inject] private SignalRService _signalR { get; set; } = null!;

    [Inject] private IClientService_UserManagment _userManagmentService { get; set; } = null!;

    private ClientDto_UserInfo? UserInfo { get; set; }

    protected override async Task OnInitializedAsync()
    {
        DataRetrieved = false;

        var getUserInfo = await _userManagmentService.GetUser();

        if (getUserInfo.Succsess)
            UserInfo = getUserInfo.Value;

        DataRetrieved = true;
    }
}