using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.GroupManagment;

namespace SafeShare.MediatR.Actions.Commands.GroupManagment;

public class MediatR_EditGroupCommand : IRequest<ObjectResult>
{
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
    public DTO_EditGroup EditGroup { get; set; }

    public MediatR_EditGroupCommand
    (
        Guid userId,
        Guid groupId,
        DTO_EditGroup editGroup
    )
    {
        UserId = userId;
        GroupId = groupId;
        EditGroup = editGroup;
    }
}