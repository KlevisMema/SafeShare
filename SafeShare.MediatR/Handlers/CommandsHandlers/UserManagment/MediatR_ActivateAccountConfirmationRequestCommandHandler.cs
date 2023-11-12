/* 
 * Defines a MediatR command handler for processing account activation confirmation requests.
 * This handler is responsible for invoking the account management service to confirm the activation of a user's account based on the provided confirmation data.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.UserManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.UserManagment;

/// <summary>
/// A MediatR command handler for processing account activation confirmation requests.
/// </summary>
public class MediatR_ActivateAccountConfirmationRequestCommandHandler :
             MediatR_GenericHandler<IAccountManagment>,
             IRequestHandler<MediatR_ActivateAccountConfirmationRequestCommand, ObjectResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_ActivateAccountConfirmationRequestCommandHandler"/> class.
    /// </summary>
    /// <param name="service">The account management service used for confirming account activations.</param>
    public MediatR_ActivateAccountConfirmationRequestCommandHandler
    (
        IAccountManagment service
    )
    : base
    (
        service
    )
    { }
    /// <summary>
    /// Handles the process of confirming a user's account activation.
    /// </summary>
    /// <param name="request">The command containing the details for the account activation confirmation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An ObjectResult with the outcome of the account activation confirmation process.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_ActivateAccountConfirmationRequestCommand request,
        CancellationToken cancellationToken
    )
    {
        var confirmAccountActivationResult = await _service.ActivateAccountConfirmation(request.DTO_ActivateAccount);

        return Util_GenericControllerResponse<bool>.ControllerResponse(confirmAccountActivationResult);
    }
}