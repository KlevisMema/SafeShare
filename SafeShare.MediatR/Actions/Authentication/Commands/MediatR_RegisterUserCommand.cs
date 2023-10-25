using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.Authentication;

namespace SafeShare.MediatR.Actions.Authentication.Commands;

public class MediatR_RegisterUserCommand : IRequest<ObjectResult>
{
    public DTO_Register Register { get; set; }

    public MediatR_RegisterUserCommand
    (
        DTO_Register register
    )
    {
        Register = register;
    }
}