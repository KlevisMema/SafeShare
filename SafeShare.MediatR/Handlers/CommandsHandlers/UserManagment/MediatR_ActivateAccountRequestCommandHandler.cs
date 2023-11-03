using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.UserManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.UserManagment;

public class MediatR_ActivateAccountRequestCommandHandler : MediatR_GenericHandler<IAccountManagment>, IRequestHandler<MediatR_ActivateAccountRequestCommand, ObjectResult>
{
    public MediatR_ActivateAccountRequestCommandHandler
    (
        IAccountManagment service
    )
    : base
    (
        service
    )
    { }

    public async Task<ObjectResult>
    Handle
    (
        MediatR_ActivateAccountRequestCommand request,
        CancellationToken cancellationToken
    )
    {
        var activateAccountResult = await _service.ActivateAccountRequest(request.Email);

        return Util_GenericControllerResponse<bool>.ControllerResponse(activateAccountResult);
    }
}