using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.MediatR.Actions.Queries.GroupManagment;
using SafeShare.DataTransormObject.GroupManagment.GroupInvitations;

namespace SafeShare.MediatR.Handlers.QueriesHandlers.GroupManagment;

public class MediatR_GetSentGroupInvitationsQueryHandler :
    MediatR_GenericHandler<IGroupManagment_GroupInvitationsRepository>,
    IRequestHandler<MediatR_GetSentGroupInvitationsQuery, ObjectResult>
{
    public MediatR_GetSentGroupInvitationsQueryHandler
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
        MediatR_GetSentGroupInvitationsQuery request,
        CancellationToken cancellationToken
    )
    {
        var getSentInvitationsResult = await _service.GetSentGroupInvitations(request.UserId);

        return Util_GenericControllerResponse<DTO_SentInvitations>.ControllerResponseList(getSentInvitationsResult);
    }
}
