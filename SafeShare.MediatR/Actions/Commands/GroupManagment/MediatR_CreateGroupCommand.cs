using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.GroupManagment;

namespace SafeShare.MediatR.Actions.Commands.GroupManagment;

public class MediatR_CreateGroupCommand : IRequest<ObjectResult>
{
    public Guid OwnerId { get; set; }
    public DTO_CreateGroup CreateGroup { get; set; }

    public MediatR_CreateGroupCommand
    (
        Guid ownerId,
        DTO_CreateGroup createGroup
    )
    {
        OwnerId = ownerId;
        CreateGroup = createGroup;
    }
}