using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.Authentication.Interfaces;
using SafeShare.MediatR.Actions.Commands.Authentication;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.Authentication;

public class MediatR_LogOutCommandHandler :
    MediatR_GenericHandler<IAUTH_Login>,
    IRequestHandler<MediatR_LogOutCommand, ObjectResult>
{
    public MediatR_LogOutCommandHandler
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
        MediatR_LogOutCommand request,
        CancellationToken cancellationToken
    )
    {
        var logOutResult = await _service.LogOut(request.Id);

        return new ObjectResult(logOutResult);
    }
}