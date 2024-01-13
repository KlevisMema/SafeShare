/*
 * Provides the database context for the SafeShare application.
 * This file configures the tables, relationships, and other database settings for the models in the application.
*/

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SafeShare.DataAccessLayer.Models.SafeShareApi;

namespace SafeShare.DataAccessLayer.Context;

/// <summary>
/// Represents the database context for the Safe Share.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
/// </remarks>
/// <param name="options">The options for configuring the database context.</param>
public class ApplicationDbContext
(
    DbContextOptions<ApplicationDbContext> options
) : IdentityDbContext<ApplicationUser>(options)
{
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
    /// <summary>
    /// Gets or sets the database table for group invitations.
    /// </summary>
    public DbSet<GroupInvitation> GroupInvitations { get; set; }
    /// <summary>
    /// Gets or sets the database table for refresh tokens.
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void
    OnModelCreating
    (
        ModelBuilder modelBuilder
    )
    {
        modelBuilder.Entity<GroupMember>().HasKey(pk => new { pk.UserId, pk.GroupId });

        modelBuilder.Entity<GroupMember>()
            .HasOne<ApplicationUser>(u => u.User)
            .WithMany(gm => gm.GroupMembers)
            .HasForeignKey(fk => fk.UserId)
            .OnDelete(deleteBehavior: DeleteBehavior.Restrict);

        modelBuilder.Entity<GroupMember>()
            .HasOne<Group>(u => u.Group)
            .WithMany(gm => gm.GroupMembers)
            .HasForeignKey(fk => fk.GroupId)
            .OnDelete(deleteBehavior: DeleteBehavior.Restrict);

        modelBuilder.Entity<ExpenseMember>().HasKey(pk => new { pk.UserId, pk.ExpenseId });

        modelBuilder.Entity<ExpenseMember>()
            .HasOne<ApplicationUser>(u => u.User)
            .WithMany(gm => gm.ExpenseMembers)
            .HasForeignKey(fk => fk.UserId)
            .OnDelete(deleteBehavior: DeleteBehavior.Restrict);

        modelBuilder.Entity<ExpenseMember>()
            .HasOne<Expense>(u => u.Expense)
            .WithMany(gm => gm.ExpenseMembers)
            .HasForeignKey(fk => fk.ExpenseId)
            .OnDelete(deleteBehavior: DeleteBehavior.Restrict);

        modelBuilder.Entity<GroupInvitation>()
            .HasOne(gi => gi.InvitingUser)
            .WithMany(u => u.SentInvitations)
            .HasForeignKey(gi => gi.InvitingUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GroupInvitation>()
            .HasOne(gi => gi.InvitedUser)
            .WithMany(u => u.ReceivedInvitations)
            .HasForeignKey(gi => gi.InvitedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}