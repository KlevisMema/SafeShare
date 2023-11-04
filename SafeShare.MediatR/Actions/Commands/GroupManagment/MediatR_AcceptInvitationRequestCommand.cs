using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.GroupManagment.GroupInvitations;

namespace SafeShare.MediatR.Actions.Commands.GroupManagment;

public class MediatR_AcceptInvitationRequestCommand : IRequest<ObjectResult>
{
    public DTO_InvitationRequestActions DTO_AcceptInvitationRequest { get; set; }

    public MediatR_AcceptInvitationRequestCommand
    (
        DTO_InvitationRequestActions dTO_AcceptInvitationRequest
    )
    {
        DTO_AcceptInvitationRequest = dTO_AcceptInvitationRequest;
    }
}