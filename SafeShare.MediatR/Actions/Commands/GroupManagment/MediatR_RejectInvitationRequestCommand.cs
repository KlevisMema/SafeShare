using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.GroupManagment.GroupInvitations;

namespace SafeShare.MediatR.Actions.Commands.GroupManagment;

public class MediatR_RejectInvitationRequestCommand : IRequest<ObjectResult>
{
    public DTO_InvitationRequestActions RejectInvitationRequest { get; set; }

    public MediatR_RejectInvitationRequestCommand
    (
        DTO_InvitationRequestActions rejectInvitationRequest
    )
    {
        RejectInvitationRequest = rejectInvitationRequest;
    }
}