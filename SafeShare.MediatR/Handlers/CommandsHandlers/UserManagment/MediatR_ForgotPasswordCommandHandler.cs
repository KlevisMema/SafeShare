using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.UserManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.UserManagment;

public class MediatR_ForgotPasswordCommandHandler : MediatR_GenericHandler<IAccountManagment>, IRequestHandler<MediatR_ForgotPasswordCommand, ObjectResult>
{
    public MediatR_ForgotPasswordCommandHandler
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
        MediatR_ForgotPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var forgotPasswordResult = await _service.ForgotPassword(request.Email);

        return Util_GenericControllerResponse<bool>.ControllerResponse(forgotPasswordResult);
    }
}