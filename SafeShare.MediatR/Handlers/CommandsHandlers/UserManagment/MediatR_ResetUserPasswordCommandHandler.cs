using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.UserManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.UserManagment;

public class MediatR_ResetUserPasswordCommandHandler : MediatR_GenericHandler<IAccountManagment>, IRequestHandler<MediatR_ResetUserPasswordCommand, ObjectResult>
{
    public MediatR_ResetUserPasswordCommandHandler
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
        MediatR_ResetUserPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var resetPasswordResult = await _service.ResetPassword(request.ResetPassword);

        return Util_GenericControllerResponse<bool>.ControllerResponse(resetPasswordResult);
    }
}