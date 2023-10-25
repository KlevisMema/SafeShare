/*
 * Represents a MediatR command for registering a user.
 * This command is expected to return an ObjectResult upon execution.
*/

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.Authentication;

namespace SafeShare.MediatR.Actions.Authentication.Commands;

/// <summary>
/// Represents a MediatR command for registering a user.
/// </summary>
public class MediatR_RegisterUserCommand : IRequest<ObjectResult>
{
    /// <summary>
    /// Gets or sets the registration details for the user.
    /// </summary>
    public DTO_Register Register { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_RegisterUserCommand"/> class.
    /// </summary>
    /// <param name="register">The registration details for the user.</param>
    public MediatR_RegisterUserCommand
    (
        DTO_Register register
    )
    {
        Register = register;
    }
}