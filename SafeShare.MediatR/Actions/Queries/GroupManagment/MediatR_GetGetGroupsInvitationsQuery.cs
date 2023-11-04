using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SafeShare.MediatR.Actions.Queries.GroupManagment;
public class MediatR_GetGetGroupsInvitationsQuery : IRequest<ObjectResult>
{
    public Guid UserId { get; set; }

    public MediatR_GetGetGroupsInvitationsQuery
    (
        Guid userId
    )
    {
        UserId = userId;
    }
}