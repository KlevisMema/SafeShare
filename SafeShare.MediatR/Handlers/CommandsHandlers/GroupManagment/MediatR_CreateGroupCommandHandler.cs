using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.GroupManagment.GroupManagment;
using SafeShare.DataTransormObject.GroupManagment;
using SafeShare.MediatR.Actions.Commands.GroupManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.GroupManagment;

public class MediatR_CreateGroupCommandHandler : MediatR_GenericHandler<IGroupManagment_GroupRepository>, IRequestHandler<MediatR_CreateGroupCommand, ObjectResult>
{
    public MediatR_CreateGroupCommandHandler
    (
        IGroupManagment_GroupRepository service
    )
    :
    base
    (
        service
    )
    { }

    public async Task<ObjectResult>
    Handle
    (
        MediatR_CreateGroupCommand request,
        CancellationToken cancellationToken
    )
    {
        var createGroupResult = await _service.CreateGroup(request.OwnerId, request.CreateGroup);

        return Util_GenericControllerResponse<DTO_GroupType>.ControllerResponse(createGroupResult);
    }
}