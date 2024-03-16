using System.ComponentModel.DataAnnotations;

namespace SafeShare.ClientDTO.Expense;

public class ClientDto_ExpenseCreate
{
    [Required]
    public Guid GroupId { get; set; }
    public string Date { get; set; } = string.Empty;
    [Required]
    [StringLength(50)]
    public string Title { get; set; } = string.Empty;
    [Required]
    [Range(0, 10000, ErrorMessage = "Invalid amount")]
    public decimal Amount { get; set; }
    [Required]
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    [StringLength(100)]
    public string EncryptedAmount { get; set; } = string.Empty;

    public string Nonce { get; set; } = string.Empty;
}