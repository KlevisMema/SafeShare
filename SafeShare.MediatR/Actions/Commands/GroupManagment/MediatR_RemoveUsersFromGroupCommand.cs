using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment;

namespace SafeShare.MediatR.Actions.Commands.GroupManagment;

public class MediatR_RemoveUsersFromGroupCommand
(
    Guid ownerId,
    Guid groupId,
    List<DTO_UsersGroupDetails> usersToRemoveFromGroup
) : IRequest<ObjectResult>
{
    public Guid OwnerId { get; } = ownerId;
    public Guid GroupId { get; } = groupId;
    public List<DTO_UsersGroupDetails> UsersToRemoveFromGroup { get; } = usersToRemoveFromGroup;
}