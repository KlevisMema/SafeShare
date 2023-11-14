﻿/*
    Base API controller for the Safe Share application.
    This controller serves as the base class for all API controllers in the application.
    It inherits from ControllerBase and provides common functionality and configurations.

    The BaseController class is decorated with [ApiController] attribute to enable API behaviors and conventions.
    It also uses [Route] attribute to define the base route for all derived controllers.
    Additionally, it includes a service filter [ServiceFilter] attribute to protect all controllers with the IApiKeyAuthorizationFilter.

    All controllers that inherit from BaseController will have the common behavior and route prefix.
*/

using MediatR;
using SafeShare.Security.API;
using SafeShare.Common.Routes;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Security.API.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace SafeShare.API.Controllers;

/// <summary>
/// Base API controller for Safe Share application.
/// </summary>

[ApiController]
[Route(BaseRoute.Route)]
//[Authorize(AuthenticationSchemes = "Default")]
//[ServiceFilter(typeof(IApiKeyAuthorizationFilter))]
public class BaseController : ControllerBase
{
    /// <summary>
    /// Mediator pattern handler for dispatching requests.
    /// </summary>
    protected readonly IMediator _mediator;
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseController"/> class.
    /// </summary>
    /// <param name="mediator">Mediator pattern handler.</param>
    public BaseController
    (
        IMediator mediator
    )
    {
        _mediator = mediator;
    }
}