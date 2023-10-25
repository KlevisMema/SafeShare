using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.Authentication.Interfaces;
using SafeShare.MediatR.Actions.Authentication.Commands;

namespace SafeShare.MediatR.Handlers.Authentication.CommandsHandler;

public class MediatR_RegisterUserCommandHandler : IRequestHandler<MediatR_RegisterUserCommand, ObjectResult>
{
    private readonly IAUTH_Register _register;

    public MediatR_RegisterUserCommandHandler
    (
        IAUTH_Register registerUser
    )
    {
        _register = registerUser;
    }

    public async Task<ObjectResult>
    Handle
    (
        MediatR_RegisterUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var registerResult = await _register.RegisterUser(request.Register);

        return Util_GenericControllerResponse<bool>.ControllerResponse(registerResult);
    }
}