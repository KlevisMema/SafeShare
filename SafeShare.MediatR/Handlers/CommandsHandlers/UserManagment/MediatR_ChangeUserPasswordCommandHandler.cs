/* 
 * Defines a MediatR command handler for changing user passwords.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.UserManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.UserManagment;

/// <summary>
/// Represents a MediatR handler that processes commands to change user passwords.
/// </summary>
public class MediatR_ChangeUserPasswordCommandHandler : MediatR_GenericHandler<IAccountManagment>, IRequestHandler<MediatR_ChangeUserPasswordCommand, ObjectResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_ChangeUserPasswordCommandHandler"/> class with the specified account management service.
    /// </summary>
    /// <param name="service">The account management service.</param>
    public MediatR_ChangeUserPasswordCommandHandler
    (
        IAccountManagment service
    )
    : base
    (
        service
    )
    { }

    /// <summary>
    /// Processes the provided command to change user passwords.
    /// </summary>
    /// <param name="request">The change password command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result indicating the success or failure of the password change operation.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_ChangeUserPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var changePasswordResult = await _service.ChangePassword(request.Id, request.UpdatePasswordDto);

        return Util_GenericControllerResponse<bool>.ControllerResponse(changePasswordResult);
    }
}