/* 
 * Defines a MediatR command handler for processing user password reset requests.
 * This handler is responsible for invoking the account management service to handle password resets based on the provided reset details.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.UserManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.UserManagment;

/// <summary>
/// A MediatR command handler for processing user password reset requests.
/// </summary>
public class MediatR_ResetUserPasswordCommandHandler :
    MediatR_GenericHandler<IAccountManagment>,
    IRequestHandler<MediatR_ResetUserPasswordCommand, ObjectResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_ResetUserPasswordCommandHandler"/> class.
    /// </summary>
    /// <param name="service">The account management service used for handling user password resets.</param>
    public MediatR_ResetUserPasswordCommandHandler
    (
        IAccountManagment service
    )
    : base
    (
        service
    )
    { }
    /// <summary>
    /// Handles the process of resetting a user's password.
    /// </summary>
    /// <param name="request">The command containing the reset password information.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An ObjectResult with the outcome of the password reset process.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_ResetUserPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var resetPasswordResult = await _service.ResetPassword(request.ResetPassword);

        return Util_GenericControllerResponse<bool>.ControllerResponse(resetPasswordResult);
    }
}