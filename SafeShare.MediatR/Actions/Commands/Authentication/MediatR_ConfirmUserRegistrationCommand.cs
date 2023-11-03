using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.Authentication;

namespace SafeShare.MediatR.Actions.Commands.Authentication;

public class MediatR_ConfirmUserRegistrationCommand : IRequest<ObjectResult>
{
    public DTO_ConfirmRegistration ConfirmRegistration { get; set; }

    public MediatR_ConfirmUserRegistrationCommand
    (
        DTO_ConfirmRegistration confirmRegistration
    )
    {
        ConfirmRegistration = confirmRegistration;
    }
}