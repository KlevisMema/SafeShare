using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.MediatR.Actions.Queries.GroupManagment;
using SafeShare.DataTransormObject.GroupManagment.GroupInvitations;


namespace SafeShare.MediatR.Handlers.QueriesHandlers.GroupManagment;

public class MediatR_GetGetGroupsInvitationsQueryHandler :
    MediatR_GenericHandler<IGroupManagment_GroupInvitationsRepository>,
    IRequestHandler<MediatR_GetGetGroupsInvitationsQuery, ObjectResult>
{
    public MediatR_GetGetGroupsInvitationsQueryHandler
    (
        IGroupManagment_GroupInvitationsRepository service
    )
    : base
    (
        service
    )
    { }

    public async Task<ObjectResult>
    Handle
    (
        MediatR_GetGetGroupsInvitationsQuery request,
        CancellationToken cancellationToken
    )
    {
        var getGroupsInvitationsResult = await _service.GetRecivedGroupsInvitations(request.UserId);

        return Util_GenericControllerResponse<DTO_RecivedInvitations>.ControllerResponseList(getGroupsInvitationsResult);
    }
}