using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.GroupManagment;
using SafeShare.Utilities.Responses;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.GroupManagment;
public class MediatR_DeleteGroupCommandHandler : MediatR_GenericHandler<IGroupManagment_GroupRepository>, IRequestHandler<MediatR_DeleteGroupCommand, ObjectResult>
{
    public MediatR_DeleteGroupCommandHandler
    (
        IGroupManagment_GroupRepository service
    )
    : base
    (
        service
    )
    { }

    public async Task<ObjectResult>
    Handle
    (
        MediatR_DeleteGroupCommand request,
        CancellationToken cancellationToken
    )
    {
        var deleteResult = await _service.DeleteGroup(request.OwnerId, request.GroupId);

        return Util_GenericControllerResponse<bool>.ControllerResponse(deleteResult);
    }
}