using SafeShare.Client.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using SafeShare.ClientDTO.AccountManagment;

namespace SafeShare.Client.Shared.Components.Account
{
    public partial class SeeProfileData
    {
        [Parameter] public ClientDto_UserInfo? UserInfo { get; set; }
        [Parameter] public bool DataRetrieved { get; set; }
        [Parameter] public SignalRService? _signalR { get; set; }

        protected override void OnInitialized()
        {
            UserInfo ??= new();

            if (_signalR is not null)
                HandleSignalR();

            base.OnInitialized();
        }

        private void
        HandleSignalR()
        {
            if (_signalR!.HubConnection is not null)
                _signalR.HubConnection.On<string>("EmailChanged", HandleEmailChanged);
        }

        private void
        HandleEmailChanged
        (
            string newEmail
        )
        {
            if (UserInfo is not null)
            {
                UserInfo.Email = newEmail;
                StateHasChanged();
            }
        }
    }
}
