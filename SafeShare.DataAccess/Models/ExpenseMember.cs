/*
 * Defines the ExpenseMember class that represents a member's participation in an expense.
 * This file contains definitions for the ExpenseMember's properties, their annotations, and relationships.
*/

using SafeShare.DataAccessLayer.BaseModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeShare.DataAccessLayer.Models;

/// <summary>
/// Represents a member's participation in an expense.
/// </summary>
public class ExpenseMember : Base
{
    /// <summary>
    /// Gets or sets the amount paid by the member for the expense.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,4)")]
    public decimal PaidShare { get; set; }
    /// <summary>
    /// Gets or sets the amount owed by the member for the expense.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,4)")]
    public decimal OwedShare { get; set; }
    /// <summary>
    /// Gets or sets the unique identifier of the associated expense.
    /// </summary>
    [Required]
    public Guid ExpenseId { get; set; }
    /// <summary>
    /// Gets or sets the navigation property representing the expense.
    /// </summary>
    public virtual Expense Expense { get; set; } = null!;
    /// <summary>
    /// Gets or sets the unique identifier of the member affected by the expense.
    /// </summary>
    [Required]
    public string UserId { get; set; } = null!;
    /// <summary>
    /// Gets or sets the navigation property representing the member affected by the expense.
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;
}