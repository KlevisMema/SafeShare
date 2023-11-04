using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.GroupManagment;
using SafeShare.Utilities.Responses;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.GroupManagment;

public class MediatR_RejectInvitationRequestCommandHandler :
    MediatR_GenericHandler<IGroupManagment_GroupInvitationsRepository>,
    IRequestHandler<MediatR_RejectInvitationRequestCommand, ObjectResult>

{
    public MediatR_RejectInvitationRequestCommandHandler
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
        MediatR_RejectInvitationRequestCommand request,
        CancellationToken cancellationToken
    )
    {
        var rejectInvitationRequest = await _service.RejectInvitation(request.RejectInvitationRequest);

        return Util_GenericControllerResponse<bool>.ControllerResponse(rejectInvitationRequest);
    }
}