/* 
 * Defines a MediatR command handler for processing requests to edit group information.
 * This handler is responsible for invoking the group management service to execute updates to a group's details, based on the provided command data.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.DataTransormObject.GroupManagment;
using SafeShare.MediatR.Actions.Commands.GroupManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.GroupManagment;

/// <summary>
/// A MediatR command handler for processing requests to edit group information.
/// </summary>
public class MediatR_EditGroupCommandHandler : 
    MediatR_GenericHandler<IGroupManagment_GroupRepository>, 
    IRequestHandler<MediatR_EditGroupCommand, ObjectResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_EditGroupCommandHandler"/> class.
    /// </summary>
    /// <param name="service">The group management service used for editing group information.</param>
    public MediatR_EditGroupCommandHandler
    (
        IGroupManagment_GroupRepository service
    )
    : base
    (
        service
    )
    { }
    /// <summary>
    /// Handles the process of editing group information.
    /// </summary>
    /// <param name="request">The command containing the details for the group edit (user ID, group ID, and new group details).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An ObjectResult with the outcome of the group editing process.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_EditGroupCommand request,
        CancellationToken cancellationToken
    )
    {
        var editResult = await _service.EditGroup(request.UserId, request.GroupId, request.EditGroup);

        return Util_GenericControllerResponse<DTO_GroupType>.ControllerResponse(editResult);
    }
}