/* 
 * Defines a MediatR command handler for confirming user registration.
 * This handler processes registration confirmation requests, using an authentication service to validate the registration data provided in the command.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.Authentication.Interfaces;
using SafeShare.MediatR.Actions.Commands.Authentication;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.Authentication;

/// <summary>
/// A MediatR command handler for confirming user registration.
/// </summary>
public class MediatR_ConfirmUserRegistrationCommandHandler :
    MediatR_GenericHandler<IAUTH_Register>,
    IRequestHandler<MediatR_ConfirmUserRegistrationCommand, ObjectResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_ConfirmUserRegistrationCommandHandler"/> class.
    /// </summary>
    /// <param name="service">The authentication service used for user registration confirmation.</param>
    public MediatR_ConfirmUserRegistrationCommandHandler
    (
        IAUTH_Register service
    )
    : base
    (
        service
    )
    { }
    /// <summary>
    /// Handles the confirmation of a user registration.
    /// </summary>
    /// <param name="request">The command containing the registration confirmation data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An ObjectResult with the outcome of the registration confirmation process.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_ConfirmUserRegistrationCommand request,
        CancellationToken cancellationToken
    )
    {
        var confirmRegistration = await _service.ConfirmRegistration(request.ConfirmRegistration);

        return Util_GenericControllerResponse<bool>.ControllerResponse(confirmRegistration);
    }
}