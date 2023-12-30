namespace SafeShare.ClientDTO.GroupManagment;

public class ClientDto_GroupType
{
    public Guid GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}