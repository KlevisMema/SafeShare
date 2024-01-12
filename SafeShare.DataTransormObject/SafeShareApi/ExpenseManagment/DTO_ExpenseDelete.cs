/*
 * Defines the data structure for deleting an expense from the system. This DTO is used to encapsulate
 * the necessary information such as the expense ID, user ID, and the amount of the expense, ensuring 
 * that the user requesting the deletion is authorized to do so and the correct expense is targeted.
 */

using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.SafeShareApi.ExpenseManagment;

/// <summary>
/// Data Transfer Object (DTO) for capturing the necessary information to delete an expense.
/// Includes validation annotations to ensure all required properties are provided.
/// </summary>
public class DTO_ExpenseDelete
{
    /// <summary>
    /// Gets or sets the unique identifier of the expense to be deleted.
    /// </summary>
    [Required]
    public Guid ExpenseId { get; set; }
    /// <summary>
    /// Gets or sets the unique identifier of the user requesting the expense deletion.
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// Gets or sets the amount of the expense, which is required for balance adjustments upon deletion.
    /// </summary>
    [Required]
    public decimal ExpenseAmount { get; set; }
    /// <summary>
    /// Gets or sets the unique identifier of the group requesting the expense deletion.
    /// </summary>
    [Required]
    public Guid GroupId { get; set; }
}