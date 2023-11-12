﻿/* 
 * Defines a MediatR command handler for processing requests to initiate a change in a user's email address.
 * This handler is responsible for invoking the account management service to handle the initial phase of changing a user's email address, often involving verification and confirmation steps.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.UserManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.UserManagment;

/// <summary>
/// A MediatR command handler for processing requests to initiate a change in a user's email address.
/// </summary>
public class MediatR_ChangeEmailAddressRequestCommandHandler :
    MediatR_GenericHandler<IAccountManagment>,
    IRequestHandler<MediatR_ChangeEmailAddressRequestCommand, ObjectResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_ChangeEmailAddressRequestCommandHandler"/> class.
    /// </summary>
    /// <param name="service">The account management service used for initiating email address changes.</param>
    public MediatR_ChangeEmailAddressRequestCommandHandler
    (
        IAccountManagment service
    )
    : base
    (
        service
    )
    { }
    /// <summary>
    /// Handles the process of initiating a change in a user's email address.
    /// </summary>
    /// <param name="request">The command containing the user ID and new email address information.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An ObjectResult with the outcome of the email address change initiation process.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_ChangeEmailAddressRequestCommand request,
        CancellationToken cancellationToken
    )
    {
        var requestChangeEmailResult = await _service.RequestChangeEmailAddress(request.UserId, request.EmailAddress);

        return Util_GenericControllerResponse<bool>.ControllerResponse(requestChangeEmailResult);
    }
}