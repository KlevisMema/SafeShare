using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SafeShare.MediatR.Actions.Queries.GroupManagment;

public class MediatR_GetSentGroupInvitationsQuery : IRequest<ObjectResult>
{
    public Guid UserId { get; set; }

    public MediatR_GetSentGroupInvitationsQuery
    (
        Guid userId
    )
    {
        UserId = userId;
    }
}