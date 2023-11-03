using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.UserManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.UserManagment;

public class MediatR_ActivateAccountConfirmationRequestCommandHandler :
             MediatR_GenericHandler<IAccountManagment>,
             IRequestHandler<MediatR_ActivateAccountConfirmationRequestCommand, ObjectResult>
{
    public MediatR_ActivateAccountConfirmationRequestCommandHandler
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
        MediatR_ActivateAccountConfirmationRequestCommand request,
        CancellationToken cancellationToken
    )
    {
        var confirmAccountActivationResult = await _service.ActivateAccountConfirmation(request.DTO_ActivateAccount);

        return Util_GenericControllerResponse<bool>.ControllerResponse(confirmAccountActivationResult);
    }
}