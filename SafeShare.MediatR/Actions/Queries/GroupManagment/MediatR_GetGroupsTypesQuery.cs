using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SafeShare.MediatR.Actions.Queries.GroupManagment;
public class MediatR_GetGroupsTypesQuery : IRequest<ObjectResult>
{
    public Guid UserId { get; set; }

    public MediatR_GetGroupsTypesQuery
    (
        Guid userId
    )
    {
        UserId = userId;
    }
}