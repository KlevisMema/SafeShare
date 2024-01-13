using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;
using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.MediatR.Actions.Commands.UserManagment;
using SafeShare.DataTransormObject.SafeShareApi.UserManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.UserManagment;

public class MediatR_UploadProfilePictureCommandHandler(
    IAccountManagment service
) : MediatR_GenericHandler<IAccountManagment>(service),
    IRequestHandler<MediatR_UploadProfilePictureCommand, ObjectResult>
{
    public async Task<ObjectResult> 
    Handle
    (
        MediatR_UploadProfilePictureCommand request, 
        CancellationToken cancellationToken
    )
    {
        var uploadProfilePicResult = await _service.UploadProfilePicture(request.UserId, request.Image);

        return Util_GenericControllerResponse<byte[]>.ControllerResponse(uploadProfilePicResult);
    }
}