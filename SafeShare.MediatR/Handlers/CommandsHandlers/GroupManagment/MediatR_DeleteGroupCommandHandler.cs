/* 
 * Defines a MediatR command handler for processing requests to delete groups within the application.
 * This handler is responsible for invoking the group management service to execute the deletion of a specified group, based on the information provided in the command.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.GroupManagment;
using SafeShare.Utilities.Responses;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.GroupManagment;

/// <summary>
/// A MediatR command handler for processing requests to delete groups within the application.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_DeleteGroupCommandHandler"/> class.
/// </remarks>
/// <param name="service">The group management service used for deleting groups.</param>
public class MediatR_DeleteGroupCommandHandler
(
    IGroupManagment_GroupRepository service
) : MediatR_GenericHandler<IGroupManagment_GroupRepository>(service),
    IRequestHandler<MediatR_DeleteGroupCommand, ObjectResult>
{
    /// <summary>
    /// Handles the process of deleting a group.
    /// </summary>
    /// <param name="request">The command containing the details for the group deletion (owner ID and group ID).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An ObjectResult with the outcome of the group deletion process.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_DeleteGroupCommand request,
        CancellationToken cancellationToken
    )
    {
        var deleteResult = await _service.DeleteGroup(request.OwnerId, request.GroupId);

        return Util_GenericControllerResponse<bool>.ControllerResponse(deleteResult);
    }
}