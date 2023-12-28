namespace SafeShare.ClientDTO.GroupManagment;

public class ClientDto_GroupDetails
{
    public string GroupName { get; set; } = string.Empty;
    public int NumberOfMembers { get; set; }
    public string LatestExpense { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string GroupAdmin { get; set; } = string.Empty;
    public DateTime GroupCreationDate { get; set; }
    public decimal TotalSpent { get; set; }
}