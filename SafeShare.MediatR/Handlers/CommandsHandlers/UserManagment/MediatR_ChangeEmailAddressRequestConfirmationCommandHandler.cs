/* 
 * Defines a MediatR command handler for processing confirmation requests for changing a user's email address.
 * This handler is responsible for invoking the account management service to confirm the email address change, based on the confirmation data provided in the command.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;
using SafeShare.DataTransormObject.Security;
using SafeShare.MediatR.Actions.Commands.UserManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.UserManagment;

/// <summary>
/// A MediatR command handler for processing confirmation requests for changing a user's email address.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_ChangeEmailAddressRequestConfirmationCommandHandler"/> class.
/// </remarks>
/// <param name="service">The account management service used for confirming email address changes.</param>
public class MediatR_ChangeEmailAddressRequestConfirmationCommandHandler
(
    IAccountManagment service
) : MediatR_GenericHandler<IAccountManagment>(service),
    IRequestHandler<MediatR_ChangeEmailAddressRequestConfirmationCommand, ObjectResult>
{
    /// <summary>
    /// Handles the process of confirming a request to change a user's email address.
    /// </summary>
    /// <param name="request">The command containing the user ID and confirmation details for the email address change.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An ObjectResult with the outcome of the email address change confirmation process.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_ChangeEmailAddressRequestConfirmationCommand request,
        CancellationToken cancellationToken
    )
    {
        var confirmChangeEmailRequestResult = await _service.ConfirmChangeEmailAddressRequest(request.UserId, request.ChangeEmailAddressConfirm);

        return Util_GenericControllerResponse<DTO_Token>.ControllerResponse(confirmChangeEmailRequestResult);
    }
}