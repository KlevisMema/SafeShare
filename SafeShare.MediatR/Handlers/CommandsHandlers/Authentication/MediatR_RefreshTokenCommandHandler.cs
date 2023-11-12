/* 
 * Defines a MediatR command handler for processing refresh token requests.
 * This handler is responsible for invoking the appropriate authentication service to handle the refresh token process, allowing users to obtain a new JWT token without re-authentication.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.Authentication.Interfaces;
using SafeShare.DataTransormObject.Security;
using SafeShare.MediatR.Actions.Commands.Authentication;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.Authentication;

/// <summary>
/// A MediatR command handler for processing refresh token requests.
/// </summary>
public class MediatR_RefreshTokenCommandHandler :
    MediatR_GenericHandler<IAUTH_RefreshToken>,
    IRequestHandler<MediatR_RefreshTokenCommand, ObjectResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_RefreshTokenCommandHandler"/> class.
    /// </summary>
    /// <param name="service">The authentication service used for refreshing JWT tokens.</param>
    public MediatR_RefreshTokenCommandHandler
    (
        IAUTH_RefreshToken service
    )
    : base
    (
        service
    )
    { }
    /// <summary>
    /// Handles the refresh token process.
    /// </summary>
    /// <param name="request">The command containing the data for the refresh token request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An ObjectResult with the outcome of the refresh token process.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_RefreshTokenCommand request,
        CancellationToken cancellationToken
    )
    {
        var refreshTokenResult = await _service.RefreshToken(request.DTO_ValidateToken);

        return Util_GenericControllerResponse<DTO_Token>.ControllerResponse(refreshTokenResult);
    }
}