/* 
 * Defines a MediatR command handler for deleting users.
*/

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.UserManagment;
using SafeShare.MediatR.Actions.Queries.UserManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.UserManagment;

/// <summary>
/// Represents a MediatR handler that processes commands to delete users.
/// </summary>
public class MediatR_DeleteUserCommandHandler : MediatR_GenericHandler<IAccountManagment>, IRequestHandler<MediatR_DeleteUserCommand, ObjectResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_DeleteUserCommandHandler"/> class with the specified account management service.
    /// </summary>
    /// <param name="service">The account management service.</param>
    public MediatR_DeleteUserCommandHandler
    (
        IAccountManagment service
    )
    : base
    (
        service
    )
    { }

    /// <summary>
    /// Processes the provided command to delete a user.
    /// </summary>
    /// <param name="request">The delete user command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result indicating the success or failure of the user deletion operation.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_DeleteUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var deleteUserResult = await _service.DeleteUser(request.Id);

        return Util_GenericControllerResponse<bool>.ControllerResponse(deleteUserResult);
    }
}