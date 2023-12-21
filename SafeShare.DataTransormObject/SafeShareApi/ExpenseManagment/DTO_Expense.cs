/*
 * Defines a data structure for transferring expense information within the application.
 * This DTO is used for operations where expense details need to be passed between different layers,
 * such as from the database to a client application. The properties are stored as byte arrays to ensure
 * that sensitive information remains encrypted during transfer. It is the responsibility of the consuming
 * code to handle the decryption of these properties before use.
 */

namespace SafeShare.DataTransormObject.SafeShareApi.ExpenseManagment;

/// <summary>
/// Data Transfer Object (DTO) representing the details of an expense.
/// This class is used to transfer expense data between layers without exposing domain models.
/// All sensitive information within the class is represented as byte arrays to maintain encryption.
/// </summary>
public class DTO_Expense
{
    /// <summary>
    /// Gets or sets the identifier for the expense.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Gets or sets the encrypted title of the expense as a byte array.
    /// </summary>
    public byte[] Title { get; set; } = null!;
    /// <summary>
    /// Gets or sets the encrypted date of the expense as a byte array.
    /// </summary>
    public byte[] Date { get; set; } = null!;
    /// <summary>
    /// Gets or sets the encrypted amount of the expense as a byte array.
    /// </summary>
    public byte[] Amount { get; set; } = null!;
    /// <summary>
    /// Gets or sets the encrypted description of the expense as a byte array.
    /// </summary>
    public byte[] Description { get; set; } = null!;
    /// <summary>
    /// Gets or sets the identifier for the group that the expense is associated with.
    /// </summary>
    public Guid GroupId { get; set; }
}