/* 
 * Defines a MediatR command handler for processing user logout requests.
 * This handler is responsible for invoking the appropriate authentication service to handle the logout process based on the user's ID.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.Authentication.Interfaces;
using SafeShare.MediatR.Actions.Commands.Authentication;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.Authentication;

/// <summary>
/// A MediatR command handler for processing user logout requests.
/// </summary>
public class MediatR_LogOutCommandHandler :
    MediatR_GenericHandler<IAUTH_Login>,
    IRequestHandler<MediatR_LogOutCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_LogOutCommandHandler"/> class.
    /// </summary>
    /// <param name="service">The authentication service used for logging out users.</param>
    public MediatR_LogOutCommandHandler
    (
        IAUTH_Login service
    )
    : base
    (
        service
    )
    { }
    /// <summary>
    /// Handles the user logout process.
    /// </summary>
    /// <param name="request">The command containing the user's ID for logout.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous logout operation.</returns>
    public async Task
    Handle
    (
        MediatR_LogOutCommand request,
        CancellationToken cancellationToken
    )
    {
        await _service.LogOut(request.Id);
    }
}