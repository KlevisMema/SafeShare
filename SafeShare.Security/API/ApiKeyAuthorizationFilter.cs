﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SafeShare.Security.API;

/// <summary>
/// Represents an API key authorization filter for application authorization.
/// </summary>
public class ApiKeyAuthorizationFilter : IAuthorizationFilter, IApiKeyAuthorizationFilter
{
    /// <summary>
    /// The API key used for authorization.
    /// </summary>
    private readonly string _apiKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyAuthorizationFilter"/> class.
    /// </summary>
    /// <param name="apiKey">The API key used for authorization.</param>
    public ApiKeyAuthorizationFilter
    (
        string apiKey
    )
    {
        _apiKey = apiKey;
    }

    /// <summary>
    /// Called when the authorization is being performed.
    /// </summary>
    /// <param name="context">The authorization filter context.</param>
    public void
    OnAuthorization
    (
        AuthorizationFilterContext context
    )
    {
        string? apiKey = context.HttpContext.Request.Headers["X-Api-Key"].FirstOrDefault();

        if (string.IsNullOrEmpty(apiKey))
            context.Result = new UnauthorizedResult();

        if (apiKey != _apiKey)
            context.Result = new UnauthorizedResult();
    }
}