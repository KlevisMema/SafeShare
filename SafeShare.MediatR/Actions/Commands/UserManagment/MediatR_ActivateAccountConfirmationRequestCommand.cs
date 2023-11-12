﻿/* 
 * Defines a MediatR command for processing account activation confirmation requests.
 * This command is intended to be handled by MediatR to activate a user's account based on the provided activation details.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.UserManagment;

namespace SafeShare.MediatR.Actions.Commands.UserManagment;

/// <summary>
/// Represents a MediatR command for handling account activation confirmation.
/// This command includes the data necessary to confirm and activate a user's account.
/// </summary>
public class MediatR_ActivateAccountConfirmationRequestCommand : IRequest<ObjectResult>
{
    /// <summary>
    /// Data transfer object containing the information required for account activation confirmation.
    /// </summary>
    public DTO_ActivateAccountConfirmation DTO_ActivateAccount { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_ActivateAccountConfirmationRequestCommand"/> class.
    /// </summary>
    /// <param name="dTO_ActivateAccount">The DTO containing account activation confirmation data.</param>
    public MediatR_ActivateAccountConfirmationRequestCommand
    (
        DTO_ActivateAccountConfirmation dTO_ActivateAccount
    )
    {
        DTO_ActivateAccount = dTO_ActivateAccount;
    }
}