using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.MediatR.Actions.Commands.GroupManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.GroupManagment;

public class MediatR_RemoveUsersFromGroupCommandHandler
(
    IGroupManagment_GroupRepository service
) : MediatR_GenericHandler<IGroupManagment_GroupRepository>(service),
    IRequestHandler<MediatR_RemoveUsersFromGroupCommand, ObjectResult>
{
    public async Task<ObjectResult> 
    Handle
    (
        MediatR_RemoveUsersFromGroupCommand request, 
        CancellationToken cancellationToken
    )
    {
        var deleteUsersFromGroupResult = await _service.RemoveUsersFromGroup(request.OwnerId, request.GroupId, request.UsersToRemoveFromGroup);

        return Util_GenericControllerResponse<bool>.ControllerResponse(deleteUsersFromGroupResult);
    }
}