using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.UserManagment;

namespace SafeShare.MediatR.Actions.Commands.UserManagment;

public class MediatR_ChangeEmailAddressRequestCommand : IRequest<ObjectResult>
{
    public Guid UserId { get; set; }
    public DTO_ChangeEmailAddressRequest EmailAddress { get; set; }

    public MediatR_ChangeEmailAddressRequestCommand
    (
        Guid userId,
        DTO_ChangeEmailAddressRequest emailAddress
    )
    {
        UserId = userId;
        EmailAddress = emailAddress;
    }
}