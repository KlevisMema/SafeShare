/* 
 * Contains helper classes for creating standardized responses for services in the SafeShare application.
 */

namespace SafeShare.Utilities.SafeShareApiKey.Helpers;

/// <summary>
/// Represents a standardized response structure for services.
/// </summary>
/// <typeparam name="T">The type of the data being returned in the response.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ServiceResponse{T}"/> class with specified data, message, and success status.
/// </remarks>
/// <param name="data">The data associated with the response.</param>
/// <param name="message">An optional message for the response.</param>
/// <param name="success">Indicates if the response is successful.</param>
public class ServiceResponse<T>(T data, string? message = null, bool success = true)
{
    /// <summary>
    /// Gets or sets the data of the response.
    /// </summary>
    public T Data { get; set; } = data;
    /// <summary>
    /// Gets or sets an optional message that may accompany the response.
    /// </summary>
    public string? Message { get; set; } = message;
    /// <summary>
    /// Gets or sets a value indicating whether the response is successful.
    /// </summary>
    public bool Success { get; set; } = success;

    /// <summary>
    /// Creates a failed service response with a specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> indicating failure.</returns>
    public static ServiceResponse<T> 
    Fail
    (
        string message
    )
    {
        return new ServiceResponse<T>(default, message, false);
    }
}
