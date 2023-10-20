namespace SafeShare.DataAccessLayer.Models;

/// <summary>
/// Represents an expense incurred within a group.
/// </summary>
public class Expense
{
    /// <summary>
    /// Gets or sets the unique identifier of the expense.
    /// </summary>
    public int ExpenseId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the group to which the expense belongs.
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the member who paid the expense.
    /// </summary>
    public string PayerUserId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the title of the expense describing what it covers.
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Gets or sets the date when the expense was made.
    /// </summary>
    public DateTime ExpenseDate { get; set; }

    /// <summary>
    /// Gets or sets the amount of the expense.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the optional description of the expense.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the navigation property representing the group to which the expense belongs.
    /// </summary>
    public Group Group { get; set; } = null!;

    /// <summary>
    /// Gets or sets the navigation property representing the member who paid the expense.
    /// </summary>
    public ApplicationUser Payer { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of members affected by the expense.
    /// </summary>
    public ICollection<ExpenseMember> ExpenseMembers { get; set; } = new List<ExpenseMember>();
}