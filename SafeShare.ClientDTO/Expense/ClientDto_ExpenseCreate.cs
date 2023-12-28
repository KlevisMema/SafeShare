using System.ComponentModel.DataAnnotations;

namespace SafeShare.ClientDTO.Expense;

public class ClientDto_ExpenseCreate
{
    [Required]
    public Guid GroupId { get; set; }
    [Required]
    public byte[] Title { get; set; } = null!;
    [Required]
    public byte[] Date { get; set; } = null!;
    [Required]
    public byte[] Amount { get; set; } = null!;
    [Required]
    public byte[] Description { get; set; } = null!;
    [Required]
    public decimal DecryptedAmount { get; set; }
}