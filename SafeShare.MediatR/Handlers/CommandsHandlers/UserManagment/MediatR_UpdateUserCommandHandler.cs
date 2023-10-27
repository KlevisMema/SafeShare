using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;
using SafeShare.DataTransormObject.UserManagment;
using SafeShare.MediatR.Actions.Commands.UserManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.UserManagment;

public class MediatR_UpdateUserCommandHandler : MediatR_GenericHandler<IAccountManagment>, IRequestHandler<MediatR_UpdateUserCommand, ObjectResult>
{
    public MediatR_UpdateUserCommandHandler
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
        MediatR_UpdateUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var updateUserResult = await _service.UpdateUser(request.Id, request.DTO_UserInfo);

        return Util_GenericControllerResponse<DTO_UserUpdatedInfo>.ControllerResponse(updateUserResult);
    }
}