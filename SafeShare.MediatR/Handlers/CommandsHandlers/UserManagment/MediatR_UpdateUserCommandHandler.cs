/* 
 * Defines a MediatR command handler for updating user information.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;
using SafeShare.DataTransormObject.UserManagment;
using SafeShare.MediatR.Actions.Commands.UserManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.UserManagment;

/// <summary>
/// Represents a MediatR handler that processes commands to update user information.
/// </summary>
public class MediatR_UpdateUserCommandHandler : MediatR_GenericHandler<IAccountManagment>, IRequestHandler<MediatR_UpdateUserCommand, ObjectResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_UpdateUserCommandHandler"/> class with the specified account management service.
    /// </summary>
    /// <param name="service">The account management service.</param>
    public MediatR_UpdateUserCommandHandler
    (
        IAccountManagment service
    )
    : base
    (
        service
    )
    { }

    /// <summary>
    /// Processes the provided command to update user information.
    /// </summary>
    /// <param name="request">The update user command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result indicating the success or failure of the user update operation.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_UpdateUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var updateUserResult = await _service.UpdateUser(request.Id, request.DTO_UserInfo);

        return Util_GenericControllerResponse<DTO_UserUpdatedInfo>.ControllerResponse(updateUserResult);
    }
}