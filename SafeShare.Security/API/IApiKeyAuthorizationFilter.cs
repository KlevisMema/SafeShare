/*
  This file contains the interface IApiKeyAuthorizationFilter, which defines the contract for an API key authorization filter for application authorization.
*/

using Microsoft.AspNetCore.Mvc.Filters;

namespace SafeShare.Security.API;

/// <summary>
/// Interface for an API key authorization filter.
/// </summary>
public interface IApiKeyAuthorizationFilter
{
    /// <summary>
    /// Called when the authorization is being performed.
    /// </summary>
    /// <param name="context">The authorization filter context.</param>
    void
    OnAuthorization
    (
        AuthorizationFilterContext context
    );
}