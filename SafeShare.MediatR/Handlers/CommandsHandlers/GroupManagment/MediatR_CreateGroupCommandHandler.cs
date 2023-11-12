/* 
 * Defines a MediatR command handler for creating new groups within the application.
 * This handler is responsible for processing group creation requests using a group management service, handling the necessary operations based on the provided command data.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.GroupManagment.GroupManagment;
using SafeShare.DataTransormObject.GroupManagment;
using SafeShare.MediatR.Actions.Commands.GroupManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.GroupManagment;

/// <summary>
/// A MediatR command handler for creating new groups within the application.
/// </summary>
public class MediatR_CreateGroupCommandHandler : 
    MediatR_GenericHandler<IGroupManagment_GroupRepository>, 
    IRequestHandler<MediatR_CreateGroupCommand, ObjectResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_CreateGroupCommandHandler"/> class.
    /// </summary>
    /// <param name="service">The group management service used for creating new groups.</param>
    public MediatR_CreateGroupCommandHandler
    (
        IGroupManagment_GroupRepository service
    )
    :
    base
    (
        service
    )
    { }
    /// <summary>
    /// Handles the process of creating a new group.
    /// </summary>
    /// <param name="request">The command containing the details for the group creation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An ObjectResult with the outcome of the group creation process.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_CreateGroupCommand request,
        CancellationToken cancellationToken
    )
    {
        var createGroupResult = await _service.CreateGroup(request.OwnerId, request.CreateGroup);

        return Util_GenericControllerResponse<DTO_GroupType>.ControllerResponse(createGroupResult);
    }
}