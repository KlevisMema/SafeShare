using SafeShare.DataAccessLayer.BaseModels;
using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataAccessLayer.Models;

/// <summary>
/// Represents a log entry for user activity monitoring.
/// </summary>
public class LogEntry : BaseId
{
    /// <summary>
    /// Gets or sets the date and time when the log entry was created.
    /// </summary>
    [Required]
    public DateTime Timestamp { get; set; } // Non-nullable since log entry must have a timestamp.

    /// <summary>
    /// Gets or sets the user ID associated with the log entry.
    /// </summary>
    [Required]
    public string UserId { get; set; } = null!; // Non-nullable since a log entry must be associated with a user.

    /// <summary>
    /// Gets or sets the details of the log entry.
    /// </summary>
    [Required, StringLength(200)]
    public string Details { get; set; } = null!; // Non-nullable since log entry must have details.
}