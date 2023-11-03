using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.DataTransormObject.GroupManagment;
using SafeShare.MediatR.Actions.Commands.GroupManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.GroupManagment;

public class MediatR_EditGroupCommandHandler : MediatR_GenericHandler<IGroupManagment_GroupRepository>, IRequestHandler<MediatR_EditGroupCommand, ObjectResult>
{
    public MediatR_EditGroupCommandHandler
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
        MediatR_EditGroupCommand request,
        CancellationToken cancellationToken
    )
    {
        var editResult = await _service.EditGroup(request.UserId, request.GroupId, request.EditGroup);

        return Util_GenericControllerResponse<DTO_GroupType>.ControllerResponse(editResult);
    }
}