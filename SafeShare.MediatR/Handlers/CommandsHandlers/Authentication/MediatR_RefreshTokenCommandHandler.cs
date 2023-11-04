using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.Authentication.Interfaces;
using SafeShare.DataTransormObject.Security;
using SafeShare.MediatR.Actions.Commands.Authentication;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.Authentication;

public class MediatR_RefreshTokenCommandHandler :
    MediatR_GenericHandler<IAUTH_RefreshToken>,
    IRequestHandler<MediatR_RefreshTokenCommand, ObjectResult>
{
    public MediatR_RefreshTokenCommandHandler
    (
        IAUTH_RefreshToken service
    )
    : base
    (
        service
    )
    { }

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