using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.UserManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.UserManagment;

public class MediatR_ChangeEmailAddressRequestCommandHandler :
    MediatR_GenericHandler<IAccountManagment>,
    IRequestHandler<MediatR_ChangeEmailAddressRequestCommand, ObjectResult>
{
    public MediatR_ChangeEmailAddressRequestCommandHandler
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
        MediatR_ChangeEmailAddressRequestCommand request,
        CancellationToken cancellationToken
    )
    {
        var requestChangeEmailResult = await _service.RequestChangeEmailAddress(request.UserId, request.EmailAddress);

        return Util_GenericControllerResponse<bool>.ControllerResponse(requestChangeEmailResult);
    }
}