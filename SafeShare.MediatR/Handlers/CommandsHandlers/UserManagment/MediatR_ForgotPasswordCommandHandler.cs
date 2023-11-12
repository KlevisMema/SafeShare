/* 
 * Defines a MediatR command handler for processing forgotten password requests.
 * This handler is responsible for invoking the account management service to handle requests for resetting passwords, typically involving sending a reset link or token to the user's email address.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.UserManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.UserManagment;

/// <summary>
/// A MediatR command handler for processing forgotten password requests.
/// </summary>
public class MediatR_ForgotPasswordCommandHandler : 
    MediatR_GenericHandler<IAccountManagment>, 
    IRequestHandler<MediatR_ForgotPasswordCommand, ObjectResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_ForgotPasswordCommandHandler"/> class.
    /// </summary>
    /// <param name="service">The account management service used for handling forgotten password requests.</param>
    public MediatR_ForgotPasswordCommandHandler
    (
        IAccountManagment service
    )
    : base
    (
        service
    )
    { }
    /// <summary>
    /// Handles the process of responding to a forgotten password request.
    /// </summary>
    /// <param name="request">The command containing the email address associated with the forgotten password.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An ObjectResult indicating the success or failure of the forgotten password request processing.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_ForgotPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var forgotPasswordResult = await _service.ForgotPassword(request.Email);

        return Util_GenericControllerResponse<bool>.ControllerResponse(forgotPasswordResult);
    }
}