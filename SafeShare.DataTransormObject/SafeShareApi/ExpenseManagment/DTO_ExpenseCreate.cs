/*
 * Represents the structure for creating a new expense within the application. This DTO includes
 * details necessary to create an expense such as the group ID, and fields for title, date,
 * amount, and description.
 */

using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.SafeShareApi.ExpenseManagment;

/// <summary>
/// Data Transfer Object (DTO) representing the necessary details to create a new expense.
/// </summary>
public class DTO_ExpenseCreate
{
    /// <summary>
    /// Gets or sets the identifier for the group associated with the new expense.
    /// </summary>
    [Required]
    public Guid GroupId { get; set; }
    /// <summary>
    /// Gets or sets the title of the expense.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Title { get; set; } = null!;
    /// <summary>
    /// Gets or sets the date of the expense.
    /// </summary>
    [Required]
    public string Date { get; set; } = null!;
    /// <summary>
    /// Gets or sets the amount of the expense.
    /// </summary>
    [Required]
    [Range(0, 10000, ErrorMessage = "Invalid amount")]
    public string Amount { get; set; } = null!;
    /// <summary>
    /// Gets or sets the description of the expense.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Description { get; set; } = null!;
}