using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.UserManagment;
using SafeShare.MediatR.Actions.Queries.UserManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.UserManagment;

public class MediatR_DeleteUserCommandHandler : MediatR_GenericHandler<IAccountManagment>, IRequestHandler<MediatR_DeleteUserCommand, ObjectResult>
{
    public MediatR_DeleteUserCommandHandler
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
        MediatR_DeleteUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var deleteUserResult = await _service.DeleteUser(request.Id);

        return Util_GenericControllerResponse<bool>.ControllerResponse(deleteUserResult);
    }
}