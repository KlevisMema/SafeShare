using SafeShare.DataAccessLayer.BaseModels;
using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataAccessLayer.Models;

/// <summary>
/// Represents an expense created by a group member.
/// </summary>
public class Expense : Base
{
    /// <summary>
    /// Gets or sets the title describing the expense.
    /// </summary>
    [Required, StringLength(100)]
    public string Title { get; set; } = null!; // Non-nullable since an expense must have a title.

    /// <summary>
    /// Gets or sets the member who encoded the expense.
    /// </summary>
    [Required]
    public string FromMemberUserId { get; set; } = null!; // Non-nullable since the encoding member must have a user ID.

    /// <summary>
    /// Gets or sets the amount of the expense.
    /// </summary>
    [Required]
    public decimal Amount { get; set; } // Non-nullable since an expense must have an amount.

    /// <summary>
    /// Gets or sets the optional description of the expense.
    /// </summary>
    [Required, StringLength(200)]
    public string? Description { get; set; } // Nullable since description is optional.

    /// <summary>
    /// Gets or sets the list of member user IDs affected by the expense.
    /// </summary>
    public virtual ICollection<ExpenseMember> ExpenseMembers { get; set; } = new List<ExpenseMember>(); // Non-nullable, initialized to an empty list.

}