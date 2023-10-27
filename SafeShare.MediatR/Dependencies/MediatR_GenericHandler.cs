/* 
 * Defines a generic MediatR handler class that provides common functionalities for handling MediatR requests.
*/

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.MediatR.Actions.Commands.UserManagment;
using SafeShare.UserManagment.Interfaces;

namespace SafeShare.MediatR.Dependencies;

/// <summary>
/// Represents a generic MediatR handler that provides common functionalities and dependency injection for service classes.
/// </summary>
/// <typeparam name="TService">The type of the service class that the handler depends on.</typeparam>
public class MediatR_GenericHandler<TService>
{
    /// <summary>
    /// Gets the service instance that the handler uses to process requests.
    /// </summary>
    protected readonly TService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_GenericHandler{TService}"/> class with the specified service.
    /// </summary>
    /// <param name="service">The service instance to be used by the handler.</param>
    public MediatR_GenericHandler
    (
        TService service
    )
    {
        _service = service;
    }
}