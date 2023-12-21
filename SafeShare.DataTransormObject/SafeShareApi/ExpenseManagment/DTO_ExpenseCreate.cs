/*
 * Represents the structure for creating a new expense within the application. This DTO includes
 * details necessary to create an expense such as the group ID, and encrypted fields for title, date,
 * amount, and description. The DecryptedAmount property is provided for the application's internal
 * logic to process financial calculations before encrypting the value for storage.
 */

using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.SafeShareApi.ExpenseManagment;

/// <summary>
/// Data Transfer Object (DTO) representing the necessary details to create a new expense.
/// It includes encrypted fields for sensitive information and a decrypted amount for internal processing.
/// </summary>
public class DTO_ExpenseCreate
{
    /// <summary>
    /// Gets or sets the identifier for the group associated with the new expense.
    /// </summary>
    [Required]
    public Guid GroupId { get; set; }
    /// <summary>
    /// Gets or sets the encrypted title of the expense as a byte array.
    /// </summary>
    [Required]
    public byte[] Title { get; set; } = null!;
    /// <summary>
    /// Gets or sets the encrypted date of the expense as a byte array.
    /// </summary>
    [Required]
    public byte[] Date { get; set; } = null!;
    /// <summary>
    /// Gets or sets the encrypted amount of the expense as a byte array.
    /// </summary>
    [Required]
    public byte[] Amount { get; set; } = null!;
    /// <summary>
    /// Gets or sets the encrypted description of the expense as a byte array.
    /// </summary>
    [Required]
    public byte[] Description { get; set; } = null!;
    /// <summary>
    /// Gets or sets the decrypted monetary value of the expense for internal calculations.
    /// This value is not encrypted as it is used for computation before storage.
    /// </summary>
    [Required]
    public decimal DecryptedAmount { get; set; }
}