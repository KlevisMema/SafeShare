using Microsoft.AspNetCore.Components;
using SafeShare.ClientServices.Interfaces;
using SafeShare.ClientDTO.AccountManagment;

namespace SafeShare.Client.Pages.Account;

public partial class Profile
{
    [Inject]
    private IClientService_UserManagment _userManagmentService { get; set; } = null!;

    private ClientDto_UserInfo? UserInfo { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var getUserInfo = await _userManagmentService.GetUser();

        if (getUserInfo.Succsess)
            UserInfo = getUserInfo.Value;
    }
}