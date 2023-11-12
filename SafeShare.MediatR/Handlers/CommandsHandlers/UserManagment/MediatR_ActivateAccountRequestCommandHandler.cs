/* 
 * Defines a MediatR command handler for processing requests to initiate the account activation process.
 * This handler is responsible for invoking the account management service to handle the initial phase of activating a user's account, typically involving sending an activation email based on the provided email address.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.UserManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.UserManagment;

/// <summary>
/// A MediatR command handler for processing requests to initiate the account activation process.
/// </summary>
public class MediatR_ActivateAccountRequestCommandHandler : 
    MediatR_GenericHandler<IAccountManagment>, 
    IRequestHandler<MediatR_ActivateAccountRequestCommand, ObjectResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_ActivateAccountRequestCommandHandler"/> class.
    /// </summary>
    /// <param name="service">The account management service used for initiating account activations.</param>
    public MediatR_ActivateAccountRequestCommandHandler
    (
        IAccountManagment service
    )
    : base
    (
        service
    )
    { }
    /// <summary>
    /// Handles the process of initiating the account activation.
    /// </summary>
    /// <param name="request">The command containing the email address for initiating the account activation process.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An ObjectResult with the outcome of the account activation initiation process.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_ActivateAccountRequestCommand request,
        CancellationToken cancellationToken
    )
    {
        var activateAccountResult = await _service.ActivateAccountRequest(request.Email);

        return Util_GenericControllerResponse<bool>.ControllerResponse(activateAccountResult);
    }
}