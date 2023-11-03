using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.UserManagment;

namespace SafeShare.MediatR.Actions.Commands.UserManagment;

public class MediatR_ChangeEmailAddressRequestCommand : IRequest<ObjectResult>
{
    public DTO_ChangeEmailAddressRequest EmailAddress { get; set; }

    public MediatR_ChangeEmailAddressRequestCommand
    (
        DTO_ChangeEmailAddressRequest emailAddress
    )
    {
        EmailAddress = emailAddress;
    }
}