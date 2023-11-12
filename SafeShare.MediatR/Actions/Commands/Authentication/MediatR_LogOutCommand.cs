/* 
 * Defines a MediatR command for handling the logout process of a user.
 * This command is used within the MediatR framework to initiate and process user logout actions.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SafeShare.MediatR.Actions.Commands.Authentication;

/// <summary>
/// Represents a MediatR command for handling user logout.
/// This command carries the user identifier to process the logout action.
/// </summary>
public class MediatR_LogOutCommand : IRequest
{
    /// <summary>
    /// The identifier of the user who is logging out.
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_LogOutCommand"/> class.
    /// </summary>
    /// <param name="id">The identifier of the user who is logging out.</param>
    public MediatR_LogOutCommand
    (
        string id
    )
    {
        Id = id;
    }
}