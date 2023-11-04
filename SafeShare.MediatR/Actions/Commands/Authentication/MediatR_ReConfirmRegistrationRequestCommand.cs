using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SafeShare.MediatR.Actions.Commands.Authentication;

public class MediatR_ReConfirmRegistrationRequestCommand : IRequest<ObjectResult>
{
    public string Email { get; set; }

    public MediatR_ReConfirmRegistrationRequestCommand
    (
        string email
    )
    {
        Email = email;
    }
}