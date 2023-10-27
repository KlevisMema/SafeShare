using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.UserManagment;

namespace SafeShare.MediatR.Actions.Commands.UserManagment;

public class MediatR_ChangeUserPasswordCommand : IRequest<ObjectResult>
{
    public Guid Id { get; set; }

    public DTO_UserChangePassword UpdatePasswordDto { get; set; }

    public MediatR_ChangeUserPasswordCommand
    (
        Guid id, 
        DTO_UserChangePassword updatePasswordDto
    )
    {
        Id = id;
        UpdatePasswordDto = updatePasswordDto;
    }
}