namespace SafeShare.ClientDTO.GroupManagment;

public class ClientDto_GroupTypes
{
    public List<ClientDto_GroupType>? GroupsCreated { get; set; }
    public List<ClientDto_GroupType>? GroupsJoined { get; set; }
    public List<ClientDto_GroupDetails>? AllGroupsDetails { get; set; }

    public int NrGroupsCreated => GroupsCreated?.Count ?? 0;
    public int NrGroupsJoined => GroupsJoined?.Count ?? 0;
    public decimal BalanceGroupsCreated => GroupsCreated?.Sum(x => x.Balance) ?? 0;
    public decimal BalanceGroupsJoined => GroupsJoined?.Sum(x => x.Balance) ?? 0;
}