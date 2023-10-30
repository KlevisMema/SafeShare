using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.DataTransormObject.GroupManagment;
using SafeShare.MediatR.Actions.Queries.GroupManagment;

namespace SafeShare.MediatR.Handlers.QueriesHandlers.GroupManagment;

public class MediatR_GetGroupsTypesQueryHandler : MediatR_GenericHandler<IGroupManagment_GroupRepository>, IRequestHandler<MediatR_GetGroupsTypesQuery, ObjectResult>
{
    public MediatR_GetGroupsTypesQueryHandler
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
        MediatR_GetGroupsTypesQuery request,
        CancellationToken cancellationToken
    )
    {
        var getGroupsTypesResult = await _service.GetGroupsTypes(request.UserId);

        return Util_GenericControllerResponse<DTO_GroupsTypes>.ControllerResponse(getGroupsTypesResult);
    }
}