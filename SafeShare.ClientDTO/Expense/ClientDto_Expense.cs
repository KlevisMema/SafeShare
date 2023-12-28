namespace SafeShare.ClientDTO.Expense;

public class ClientDto_Expense
{
    public Guid Id { get; set; }
    public byte[] Title { get; set; } = null!;
    public byte[] Date { get; set; } = null!;
    public byte[] Amount { get; set; } = null!;
    public byte[] Description { get; set; } = null!;
    public Guid GroupId { get; set; }
}