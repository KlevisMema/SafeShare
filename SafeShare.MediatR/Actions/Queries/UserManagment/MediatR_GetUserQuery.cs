using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SafeShare.MediatR.Actions.Queries.UserManagment;
public class MediatR_GetUserQuery : IRequest<ObjectResult>
{
    public Guid Id { get; set; }

    public MediatR_GetUserQuery
    (
        Guid id
    )
    {
        Id = id;
    }
}