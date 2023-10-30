using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SafeShare.MediatR.Actions.Queries.GroupManagment;

public class MediatR_GetGroupDetailsQuery : IRequest<ObjectResult>
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }

    public MediatR_GetGroupDetailsQuery
    (
        Guid userId,
        Guid groupId
    )
    {
        UserId = userId;
        GroupId = groupId;
    }
}