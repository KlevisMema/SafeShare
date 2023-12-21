/*
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SafeShare.ClientServerShared.Routes;

namespace SafeShare.API.Controllers;

/// <summary>
/// Base API controller for Safe Share application.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BaseController"/> class.
/// </remarks>
/// <param name="mediator">Mediator pattern handler.</param>

[ApiController]
[Route(BaseRoute.Route)]
[Authorize(AuthenticationSchemes = "Default")]
public class BaseController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Mediator pattern handler for dispatching requests.
    /// </summary>
    protected readonly IMediator _mediator = mediator;
}