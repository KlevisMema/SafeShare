    namespace SafeShare.DataAccessLayer.Models;

/// <summary>
/// Represents a member's participation in an expense.
/// </summary>
public class ExpenseMember
{
    /// <summary>
    /// Gets or sets the unique identifier of the expense member.
    /// </summary>
    public int ExpenseMemberId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the associated expense.
    /// </summary>
    public int ExpenseId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the member affected by the expense.
    /// </summary>
    public string MemberUserId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the amount paid by the member for the expense.
    /// </summary>
    public decimal PaidShare { get; set; }

    /// <summary>
    /// Gets or sets the amount owed by the member for the expense.
    /// </summary>
    public decimal OwedShare { get; set; }

    /// <summary>
    /// Gets or sets the navigation property representing the expense.
    /// </summary>
    public Expense Expense { get; set; } = null!;

    /// <summary>
    /// Gets or sets the navigation property representing the member affected by the expense.
    /// </summary>
    public ApplicationUser Member { get; set; } = null!;
}