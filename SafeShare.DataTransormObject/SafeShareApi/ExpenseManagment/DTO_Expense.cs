/*
 * Defines a data structure for transferring expense information within the application.
 * This DTO is used for operations where expense details need to be passed between different layers,
 * such as from the database to a client application. 
 */

namespace SafeShare.DataTransormObject.SafeShareApi.ExpenseManagment;

/// <summary>
/// Data Transfer Object (DTO) representing the details of an expense.
/// This class is used to transfer expense data between layers without exposing domain models.
/// </summary>
public class DTO_Expense
{
    /// <summary>
    /// Gets or sets the identifier for the expense.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Gets or sets the  title of the expense.
    /// </summary>
    public string Title { get; set; } = null!;
    /// <summary>
    /// Gets or sets the  date of the expense.
    /// </summary>
    public string Date { get; set; } = null!;
    /// <summary>
    /// Gets or sets the  amount of the expense.
    /// </summary>
    public string Amount { get; set; } = null!;
    /// <summary>
    /// Gets or sets the  description of the expense.
    /// </summary>
    public string Description { get; set; } = null!;
    /// <summary>
    /// Gets or sets the identifier for the group that the expense is associated with.
    /// </summary>
    public Guid GroupId { get; set; }
    /// <summary>
    /// Gets or sets the creator of the expense.
    /// </summary>
    public bool CreatedByMe { get; set; }
    /// <summary>
    /// Gets or sets the identifier name of the expense.
    /// </summary>
    public string CreatorOfExpense { get; set; } = string.Empty!;
}