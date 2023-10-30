using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.DataTransormObject.GroupManagment;
using SafeShare.MediatR.Actions.Queries.GroupManagment;

namespace SafeShare.MediatR.Handlers.QueriesHandlers.GroupManagment;
public class MediatR_GetGroupDetailsQueryHandler : MediatR_GenericHandler<IGroupManagment_GroupRepository>, IRequestHandler<MediatR_GetGroupDetailsQuery, ObjectResult>
{
    public MediatR_GetGroupDetailsQueryHandler
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
        MediatR_GetGroupDetailsQuery request,
        CancellationToken cancellationToken
    )
    {
        var getGroupDetailsResult = await _service.GetGroupDetails(request.UserId, request.GroupId);

        return Util_GenericControllerResponse<DTO_GroupDetails>.ControllerResponse(getGroupDetailsResult);
    }
}