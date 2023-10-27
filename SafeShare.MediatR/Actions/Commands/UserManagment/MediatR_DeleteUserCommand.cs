using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.UserManagment;

namespace SafeShare.MediatR.Actions.Commands.UserManagment;

public class MediatR_DeleteUserCommand : IRequest<ObjectResult>
{
    public Guid Id { get; set; }

    public MediatR_DeleteUserCommand
    (
        Guid id
    )
    {
        Id = id;
    }
}