using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.Authentication.Interfaces;
using SafeShare.DataTransormObject.Authentication;
using SafeShare.MediatR.Actions.Commands.Authentication;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.Authentication;

public class MediatR_ConfirmLoginUserCommandHandler : MediatR_GenericHandler<IAUTH_Login>, IRequestHandler<MediatR_ConfirmLoginUserCommand, ObjectResult>
{
    public MediatR_ConfirmLoginUserCommandHandler
    (
        IAUTH_Login service
    )
    : base
    (
        service
    )
    { }

    public async Task<ObjectResult>
    Handle
    (
        MediatR_ConfirmLoginUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var confirmLoginResult = await _service.ConfirmLogin(request.UserId, request.OTP);

        return Util_GenericControllerResponse<DTO_LoginResult>.ControllerResponse(confirmLoginResult);
    }
}