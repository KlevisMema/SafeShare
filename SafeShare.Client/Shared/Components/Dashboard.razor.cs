using System.Linq;
using SafeShare.Client.Internal;
using SafeShare.Client.Pages.Group;
using Microsoft.AspNetCore.Components;
using SafeShare.ClientDTO.GroupManagment;
using SafeShare.ClientServices.GroupManagment;
using Microsoft.AspNetCore.SignalR.Client;

namespace SafeShare.Client.Shared.Components
{
    public partial class Dashboard
    {
        [Inject] private AppState _appState { get; set; } = null!;
        [Inject] private NavigationManager _navigationManager { get; set; } = null!;

        [Parameter]
        public List<ClientDto_GroupDetails> GroupsDetails { get; set; }

        [Parameter]
        public int NrGroupsCreated { get; set; }

        [Parameter]
        public int NrGroupsJoined { get; set; }

        [Parameter]
        public decimal BalanceGroupsJoined { get; set; }

        [Parameter]
        public decimal BalanceGroupsCreated { get; set; }

        [Parameter]
        public bool DataRetrieved { get; set; }

        private string _searchString;
        private bool playAnimationOfRefeshData;


        protected override async Task
        OnInitializedAsync()
        {
            _appState.OnGroupDeleted += HandleGroupDeleted;
            _appState.OnNewGroupCreated += HandleNewGroupCreated;
            _appState.OnRemovedFromGroup += HandleRemovedFromGroup;
            _appState.OnGroupInvitationAccepted += HandleGroupInvitationAccepted;
            _appState.OnGroupDetails += HandleGroupDetails;

            await base.OnInitializedAsync();
        }

        private void
        RefreshData()
        {
            _navigationManager.NavigateTo("/Dashboard", true);
        }

        private void
        HandleRemovedFromGroup
        (
            Guid groupId
        )
        {
            var groupToDeleteFromDom = GroupsDetails.FirstOrDefault(x => x.GroupId == groupId);

            if (groupToDeleteFromDom is not null)
                GroupsDetails.Remove(groupToDeleteFromDom);

            NrGroupsJoined--;

            StateHasChanged();
        }

        private void
        HandleNewGroupCreated
        (
            ClientDto_GroupType? newGroup
        )
        {
            if (newGroup != null)
            {
                NrGroupsCreated++;
                StateHasChanged();
            }
        }

        private void
        HandleGroupDeleted
        (
            Guid groupId
        )
        {
            var groupToDeleteFromDom = GroupsDetails.FirstOrDefault(x => x.GroupId == groupId);

            if (groupToDeleteFromDom != null)
                GroupsDetails.Remove(groupToDeleteFromDom);

            NrGroupsCreated--;
            StateHasChanged();
        }

        private void
        HandleGroupDetails
        (
            ClientDto_GroupDetails? groupDetails
        )
        {
            if (groupDetails is not null)
                GroupsDetails.Add(groupDetails);

            StateHasChanged();
        }

        private void
        HandleGroupInvitationAccepted
        (
            ClientDto_GroupType? group
        )
        {
            if (group != null)
            {
                NrGroupsJoined++;
                StateHasChanged();
            }
        }

        public void
        Dispose()
        {
            _appState.OnGroupDetails -= HandleGroupDetails;
            _appState.OnGroupDeleted -= HandleGroupDeleted;
            _appState.OnNewGroupCreated -= HandleNewGroupCreated;
            _appState.OnRemovedFromGroup += HandleRemovedFromGroup;
            _appState.OnGroupInvitationAccepted -= HandleGroupInvitationAccepted;
        }

        private Func<ClientDto_GroupDetails, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (x.GroupName.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.GroupAdmin.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.Description.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if ($"{x.NumberOfMembers}".Contains(_searchString))
                return true;

            return false;
        };

    }
}