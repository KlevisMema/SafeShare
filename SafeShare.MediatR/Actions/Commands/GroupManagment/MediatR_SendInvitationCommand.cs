using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.GroupManagment.GroupInvitations;

namespace SafeShare.MediatR.Actions.Commands.GroupManagment;

public class MediatR_SendInvitationCommand : IRequest<ObjectResult>
{
    public DTO_SendInvitationRequest DTO_SendInvitation { get; set; }

    public MediatR_SendInvitationCommand
    (
        DTO_SendInvitationRequest dTO_SendInvitation
    )
    {
        DTO_SendInvitation = dTO_SendInvitation;
    }
}