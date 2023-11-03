using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.UserManagment;

namespace SafeShare.MediatR.Actions.Commands.UserManagment;

public class MediatR_ResetUserPasswordCommand : IRequest<ObjectResult>
{
    public DTO_ResetPassword ResetPassword { get; set; }

    public MediatR_ResetUserPasswordCommand
    (
        DTO_ResetPassword resetPassword
    )
    {
        ResetPassword = resetPassword;
    }
}