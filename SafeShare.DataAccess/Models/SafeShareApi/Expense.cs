/*
 * Defines the Expense class that represents expenses created by group members.
 * This file contains definitions for the Expense's properties, their annotations, and relationships.
 */

using SafeShare.DataAccessLayer.BaseModels;
using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataAccessLayer.Models.SafeShareApi;

/// <summary>
/// Represents an expense created by a group member.
/// </summary>
public class Expense : Base
{
    /// <summary>
    /// Gets or sets the primary key of the expense
    /// </summary>
    [Key]
    public Guid Id { get; set; }
    /// <summary>
    /// Encrypted title of the expense.
    /// </summary>
    [Required]
    public string Title { get; set; } = null!;
    /// <summary>
    /// Encrypted date the expense was made.
    /// </summary>
    [Required]
    public string Date { get; set; } = null!;
    /// <summary>
    /// Encrypted amount of the expense.
    /// </summary>
    [Required]
    [Range(0, 10000, ErrorMessage = "Invalid amount")]
    public string Amount { get; set; } = null!;
    /// <summary>
    /// Encrypted (optional) description of the expense.
    /// </summary>
    [Required]
    public string Desc { get; set; } = null!;
    /// <summary>
    /// Gets or sets the Group ID associated with the expense.
    /// </summary>
    [Required]
    public Guid GroupId { get; set; }
    /// <summary>
    /// Navigation property for the group associated with the expense.
    /// </summary>
    public virtual Group Group { get; set; } = null!;
    /// <summary>
    /// Navigation property for the list of members affected by the expense.
    /// </summary>
    public virtual ICollection<ExpenseMember> ExpenseMembers { get; set; } = null!;

}