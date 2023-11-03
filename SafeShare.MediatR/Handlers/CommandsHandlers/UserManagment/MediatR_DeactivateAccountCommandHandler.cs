/* 
 * Defines a MediatR command handler for deactivating users.
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
/// Represents a MediatR handler that processes commands to deactivate users.
/// </summary>
public class MediatR_DeactivateAccountCommandHandler : MediatR_GenericHandler<IAccountManagment>, IRequestHandler<MediatR_DeactivateAccountCommand, ObjectResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_DeactivateAccountCommandHandler"/> class with the specified account management service.
    /// </summary>
    /// <param name="service">The account management service.</param>
    public MediatR_DeactivateAccountCommandHandler
    (
        IAccountManagment service
    )
    : base
    (
        service
    )
    { }

    /// <summary>
    /// Processes the provided command to deactivate a user.
    /// </summary>
    /// <param name="request">The deactivated user command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result indicating the success or failure of the user deactivation operation.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_DeactivateAccountCommand request,
        CancellationToken cancellationToken
    )
    {
        var deleteUserResult = await _service.DeactivateAccount(request.Id, request.DeactivateAccount);

        return Util_GenericControllerResponse<bool>.ControllerResponse(deleteUserResult);
    }
}