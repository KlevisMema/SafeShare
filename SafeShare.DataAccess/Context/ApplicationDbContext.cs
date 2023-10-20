using Microsoft.EntityFrameworkCore;
using SafeShare.DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SafeShare.DataAccessLayer.Context;

/// <summary>
/// Represents the database context for the Safe Share.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
    /// </summary>
    /// <param name="options">The options for configuring the database context.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }

    /// <summary>
    /// Gets or sets the database table for labels.
    /// </summary>
    public DbSet<Group> Groups { get; set; }

    /// <summary>
    /// Gets or sets the database table for Expenses.
    /// </summary>
    public DbSet<Expense> Expenses { get; set; }

    /// <summary>
    /// Gets or sets the database table for Log Entries.
    /// </summary>
    public DbSet<LogEntry> LogEntries { get; set; }

    /// <summary>
    /// Gets or sets the database table for Group Members.
    /// </summary>
    public DbSet<GroupMember> GroupMembers { get; set; }

    /// <summary>
    /// Gets or sets the database table for Expense Members.
    /// </summary>
    public DbSet<ExpenseMember> ExpenseMembers { get; set; }
}