namespace SafeShare.ClientDTO.Expense;

public class ClientDto_Expense
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "N/A";
    public string Date { get; set; } = "N/A";
    public string Amount { get; set; } = "N/A";
    public string Description { get; set; } = "N/A";
    public Guid GroupId { get; set; }
    public bool CreatedByMe { get; set; } = false;
    public string CreatorOfExpense { get; set; } = "N/A";
}