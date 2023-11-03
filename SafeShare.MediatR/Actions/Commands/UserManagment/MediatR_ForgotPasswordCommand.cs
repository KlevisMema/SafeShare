using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SafeShare.MediatR.Actions.Commands.UserManagment;

public class MediatR_ForgotPasswordCommand : IRequest<ObjectResult>
{
    public string Email { get; set; }

    public MediatR_ForgotPasswordCommand
    (
        string email
    )
    {
        Email = email;
    }
}