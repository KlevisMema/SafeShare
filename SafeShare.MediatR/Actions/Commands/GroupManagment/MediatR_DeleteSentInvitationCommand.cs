using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.GroupManagment.GroupInvitations;

namespace SafeShare.MediatR.Actions.Commands.GroupManagment;

public class MediatR_DeleteSentInvitationCommand : IRequest<ObjectResult>
{
    public DTO_InvitationRequestActions DeleteInvitationRequest { get; set; }

    public MediatR_DeleteSentInvitationCommand
    (
        DTO_InvitationRequestActions deleteInvitationRequest
    )
    {
        DeleteInvitationRequest = deleteInvitationRequest;
    }
}