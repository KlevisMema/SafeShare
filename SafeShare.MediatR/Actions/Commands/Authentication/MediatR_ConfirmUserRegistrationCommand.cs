/* 
 * Defines a MediatR command for confirming a user's registration.
 * This command is intended for use within MediatR handlers to facilitate the process of user registration confirmation.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.Authentication;

namespace SafeShare.MediatR.Actions.Commands.Authentication;

/// <summary>
/// Represents a MediatR command for confirming a user's registration.
/// This command carries the necessary data for the registration confirmation process.
/// </summary>
public class MediatR_ConfirmUserRegistrationCommand : IRequest<ObjectResult>
{
    /// <summary>
    /// Data transfer object containing the information required for confirming user registration.
    /// </summary>
    public DTO_ConfirmRegistration ConfirmRegistration { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_ConfirmUserRegistrationCommand"/> class.
    /// </summary>
    /// <param name="confirmRegistration">The DTO containing registration confirmation data.</param>
    public MediatR_ConfirmUserRegistrationCommand
    (
        DTO_ConfirmRegistration confirmRegistration
    )
    {
        ConfirmRegistration = confirmRegistration;
    }
}