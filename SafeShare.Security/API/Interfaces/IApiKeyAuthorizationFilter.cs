/* 
 * Defines the interface IApiKeyAuthorizationFilter, outlining the contract for implementing an API key authorization filter.
 * This interface is intended to provide a method for handling authorization based on API keys, ensuring secure access to application resources.
 */

using Microsoft.AspNetCore.Mvc.Filters;

namespace SafeShare.Security.API.Interfaces;

/// <summary>
/// Interface defining the contract for an API key authorization filter.
/// This filter is responsible for authorizing requests based on API keys.
/// </summary>
public interface IApiKeyAuthorizationFilter
{
    /// <summary>
    /// Method called during the authorization process of a request.
    /// Implement this method to add custom logic for authorizing requests based on API keys.
    /// </summary>
    /// <param name="context">The context for the authorization filter, providing access to request details.</param>
    void
    OnAuthorization
    (
        AuthorizationFilterContext context
    );
}