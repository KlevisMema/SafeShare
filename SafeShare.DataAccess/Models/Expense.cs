using SafeShare.DataAccessLayer.BaseModels;

namespace SafeShare.DataAccessLayer.Models;

/// <summary>
/// Represents an expense created by a group member.
/// </summary>
public class Expense : Base
{
    /// <summary>
    /// Gets or sets the title describing the expense.
    /// </summary>
    public string Title { get; set; } = null!; // Non-nullable since an expense must have a title.

    /// <summary>
    /// Gets or sets the member who encoded the expense.
    /// </summary>
    public string FromMemberUserId { get; set; } = null!; // Non-nullable since the encoding member must have a user ID.

    /// <summary>
    /// Gets or sets the list of member user IDs affected by the expense.
    /// </summary>
    public List<string> ToMemberUserIds { get; set; } = new List<string>(); // Non-nullable, initialized to an empty list.

    /// <summary>
    /// Gets or sets the amount of the expense.
    /// </summary>
    public decimal Amount { get; set; } // Non-nullable since an expense must have an amount.

    /// <summary>
    /// Gets or sets the optional description of the expense.
    /// </summary>
    public string? Description { get; set; } // Nullable since description is optional.
}