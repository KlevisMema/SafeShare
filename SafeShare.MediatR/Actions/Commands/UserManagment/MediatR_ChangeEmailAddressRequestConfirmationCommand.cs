using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.UserManagment;

namespace SafeShare.MediatR.Actions.Commands.UserManagment;

public class MediatR_ChangeEmailAddressRequestConfirmationCommand : IRequest<ObjectResult>
{
    public Guid UserId { get; set; }
    public DTO_ChangeEmailAddressRequestConfirm ChangeEmailAddressConfirm { get; set; }

    public MediatR_ChangeEmailAddressRequestConfirmationCommand
    (
        Guid userId,
        DTO_ChangeEmailAddressRequestConfirm changeEmailAddressConfirm
    )

    {
        UserId = userId;
        ChangeEmailAddressConfirm = changeEmailAddressConfirm;
    }
}