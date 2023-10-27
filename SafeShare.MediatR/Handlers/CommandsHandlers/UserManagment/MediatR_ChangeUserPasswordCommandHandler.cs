using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.UserManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.UserManagment;

public class MediatR_ChangeUserPasswordCommandHandler : MediatR_GenericHandler<IAccountManagment>, IRequestHandler<MediatR_ChangeUserPasswordCommand, ObjectResult>
{
    public MediatR_ChangeUserPasswordCommandHandler
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
        MediatR_ChangeUserPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var changePasswordResult = await _service.ChangePassword(request.Id, request.UpdatePasswordDto);

        return Util_GenericControllerResponse<bool>.ControllerResponse(changePasswordResult);
    }
}