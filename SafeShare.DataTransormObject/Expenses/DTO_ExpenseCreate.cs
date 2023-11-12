namespace SafeShare.DataTransormObject.Expenses;
public class DTO_ExpenseCreate
{
    public Guid GroupId { get; set; }
    public byte[] Title { get; set; } = null!;
    public byte[] Date { get; set; } = null!;
    public byte[] Amount { get; set; } = null!;
    public byte[] Description { get; set; } = null!;
}