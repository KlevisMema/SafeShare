using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.GroupManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.GroupManagment;

public class MediatR_DeleteSentInvitationCommandHandler :
    MediatR_GenericHandler<IGroupManagment_GroupInvitationsRepository>,
    IRequestHandler<MediatR_DeleteSentInvitationCommand, ObjectResult>
{
    public MediatR_DeleteSentInvitationCommandHandler
    (
        IGroupManagment_GroupInvitationsRepository service
    )
    : base
    (
        service
    )
    { }

    public async Task<ObjectResult>
    Handle
    (
        MediatR_DeleteSentInvitationCommand request,
        CancellationToken cancellationToken
    )
    {
        var deleteInvitationResult = await _service.DeleteSentInvitation(request.DeleteInvitationRequest);

        return Util_GenericControllerResponse<bool>.ControllerResponse(deleteInvitationResult);
    }
}