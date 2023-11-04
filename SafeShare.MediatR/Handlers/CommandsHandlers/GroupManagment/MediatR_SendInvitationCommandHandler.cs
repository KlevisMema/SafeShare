using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.MediatR.Actions.Commands.GroupManagment;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.GroupManagment;

public class MediatR_SendInvitationCommandHandler : MediatR_GenericHandler<IGroupManagment_GroupInvitationsRepository>,
    IRequestHandler<MediatR_SendInvitationCommand, ObjectResult>
{
    public MediatR_SendInvitationCommandHandler
    (
        IGroupManagment_GroupInvitationsRepository service
    )
    : base(service)
    { }

    public async Task<ObjectResult>
    Handle
    (
        MediatR_SendInvitationCommand request,
        CancellationToken cancellationToken
    )
    {
        var sendInvitationResult = await _service.SendInvitation(request.DTO_SendInvitation);

        return Util_GenericControllerResponse<bool>.ControllerResponse(sendInvitationResult);
    }
}