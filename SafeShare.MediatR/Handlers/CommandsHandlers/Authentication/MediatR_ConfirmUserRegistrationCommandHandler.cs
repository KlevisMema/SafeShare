using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.Authentication.Interfaces;
using SafeShare.MediatR.Actions.Commands.Authentication;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.Authentication;

public class MediatR_ConfirmUserRegistrationCommandHandler :
    MediatR_GenericHandler<IAUTH_Register>,
    IRequestHandler<MediatR_ConfirmUserRegistrationCommand, ObjectResult>
{
    public MediatR_ConfirmUserRegistrationCommandHandler
    (
        IAUTH_Register service
    )
    : base
    (
        service
    )
    { }

    public async Task<ObjectResult>
    Handle
    (
        MediatR_ConfirmUserRegistrationCommand request,
        CancellationToken cancellationToken
    )
    {
        var confirmRegistration = await _service.ConfirmRegistration(request.ConfirmRegistration);

        return Util_GenericControllerResponse<bool>.ControllerResponse(confirmRegistration);
    }
}