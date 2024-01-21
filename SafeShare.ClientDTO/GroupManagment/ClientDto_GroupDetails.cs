namespace SafeShare.ClientDTO.GroupManagment;

public class ClientDto_GroupDetails
{
    public Guid GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public int NumberOfMembers { get; set; }
    public string Description { get; set; } = string.Empty;
    public string GroupAdmin { get; set; } = string.Empty;
    public DateTime GroupCreationDate { get; set; }
    public bool ImAdmin { get; set; }
    public List<ClientDto_UsersGroupDetails> UsersGroups { get; set; } = [];
}