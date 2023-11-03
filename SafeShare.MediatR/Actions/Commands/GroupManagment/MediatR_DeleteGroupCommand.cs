using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SafeShare.MediatR.Actions.Commands.GroupManagment;

public class MediatR_DeleteGroupCommand : IRequest<ObjectResult>
{
    public Guid OwnerId { get; set; }
    public Guid GroupId { get; set; }

    public MediatR_DeleteGroupCommand
    (
        Guid ownerId,
        Guid groupId
    )
    {
        OwnerId = ownerId;
        GroupId = groupId;
    }
}