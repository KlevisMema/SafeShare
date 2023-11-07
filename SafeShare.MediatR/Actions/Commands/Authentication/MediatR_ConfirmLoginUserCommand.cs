using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SafeShare.MediatR.Actions.Commands.Authentication;

public class MediatR_ConfirmLoginUserCommand : IRequest<ObjectResult>
{
    public Guid UserId { get; set; }
    public string OTP { get; set; }

    public MediatR_ConfirmLoginUserCommand
    (
        string oTP, 
        Guid userId
    )
    {
        OTP = oTP;
        UserId = userId;
    }
}