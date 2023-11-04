using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.GroupManagment;
using SafeShare.Utilities.Responses;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.GroupManagment;

public class MediatR_AcceptInvitationRequestCommandHandler :
    MediatR_GenericHandler<IGroupManagment_GroupInvitationsRepository>,
    IRequestHandler<MediatR_AcceptInvitationRequestCommand, ObjectResult>
{
    public MediatR_AcceptInvitationRequestCommandHandler
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
        MediatR_AcceptInvitationRequestCommand request,
        CancellationToken cancellationToken
    )
    {
        var acceptInvitationRequest = await _service.AcceptInvitation(request.DTO_AcceptInvitationRequest);

        return Util_GenericControllerResponse<bool>.ControllerResponse(acceptInvitationRequest);
    }
}