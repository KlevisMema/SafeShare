﻿/* 
 * Defines a MediatR command for handling the token refresh process.
 * This command is intended for use within MediatR handlers to facilitate the refreshing of JWT tokens, ensuring continuous and secure authentication.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.Security;

namespace SafeShare.MediatR.Actions.Commands.Authentication;

/// <summary>
/// Represents a MediatR command for refreshing a JWT (JSON Web Token).
/// This command includes the necessary data for the token refresh process.
/// </summary>
public class MediatR_RefreshTokenCommand : IRequest<ObjectResult>
{
    /// <summary>
    /// Data transfer object containing the information required for token validation and refresh.
    /// </summary>
    public DTO_ValidateToken DTO_ValidateToken { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_RefreshTokenCommand"/> class.
    /// </summary>
    /// <param name="dTO_ValidateToken">The DTO containing token validation and refresh data.</param>
    public MediatR_RefreshTokenCommand
    (
        DTO_ValidateToken dTO_ValidateToken
    )
    {
        DTO_ValidateToken = dTO_ValidateToken;
    }
}