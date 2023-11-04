using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.Authentication.Interfaces;
using SafeShare.MediatR.Actions.Commands.Authentication;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.Authentication;

public class MediatR_ReConfirmRegistrationRequestCommandHandler :
    MediatR_GenericHandler<IAUTH_Register>,
    IRequestHandler<MediatR_ReConfirmRegistrationRequestCommand, ObjectResult>
{
    public MediatR_ReConfirmRegistrationRequestCommandHandler
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
        MediatR_ReConfirmRegistrationRequestCommand request,
        CancellationToken cancellationToken
    )
    {
        var reregistrationRequestResult = await _service.ReConfirmRegistrationRequest(request.Email);

        return Util_GenericControllerResponse<bool>.ControllerResponse(reregistrationRequestResult);
    }
}