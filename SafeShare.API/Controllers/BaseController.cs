/*
    Base API controller for the Safe Share application.
    This controller serves as the base class for all API controllers in the application.
    It inherits from ControllerBase and provides common functionality and configurations.

    The BaseController class is decorated with [ApiController] attribute to enable API behaviors and conventions.
    It also uses [Route] attribute to define the base route for all derived controllers.
    Additionally, it includes a service filter [ServiceFilter] attribute to protect all controllers with the IApiKeyAuthorizationFilter.

    All controllers that inherit from BaseController will have the common behavior and route prefix.
*/

using SafeShare.Security.API;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Security.API.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace SafeShare.API.Controllers;

/// <summary>
/// Base API controller for Safe Share application.
/// </summary>

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "Default")]
[ServiceFilter(typeof(IApiKeyAuthorizationFilter))]
public class BaseController : ControllerBase
{
}