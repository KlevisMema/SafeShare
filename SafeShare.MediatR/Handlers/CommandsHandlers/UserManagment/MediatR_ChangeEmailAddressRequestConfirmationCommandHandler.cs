using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.UserManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.UserManagment;

public class MediatR_ChangeEmailAddressRequestConfirmationCommandHandler :
    MediatR_GenericHandler<IAccountManagment>,
    IRequestHandler<MediatR_ChangeEmailAddressRequestConfirmationCommand, ObjectResult>
{
    public MediatR_ChangeEmailAddressRequestConfirmationCommandHandler(IAccountManagment service) : base(service)
    {
    }

    public async Task<ObjectResult>
    Handle
    (
        MediatR_ChangeEmailAddressRequestConfirmationCommand request,
        CancellationToken cancellationToken
    )
    {
        var confirmChangeEmailRequestResult = await _service.ConfirmChangeEmailAddressRequest(request.ChangeEmailAddressConfirm);

        return Util_GenericControllerResponse<bool>.ControllerResponse(confirmChangeEmailRequestResult);
    }
}