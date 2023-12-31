using Microsoft.AspNetCore.Components;
using SafeShare.ClientDTO.GroupManagment;
using static MudBlazor.CategoryTypes;

namespace SafeShare.Client.Shared.Components
{
    public partial class Dashboard
    {
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

            if ($"{x.TotalSpent}$".Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if ($"{x.NumberOfMembers}".Contains(_searchString))
                return true;

            return false;
        };
    }
}
