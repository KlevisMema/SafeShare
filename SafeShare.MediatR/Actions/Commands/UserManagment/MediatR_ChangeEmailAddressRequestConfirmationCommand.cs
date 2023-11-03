using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.UserManagment;

namespace SafeShare.MediatR.Actions.Commands.UserManagment;

public class MediatR_ChangeEmailAddressRequestConfirmationCommand : IRequest<ObjectResult>
{
    public DTO_ChangeEmailAddressRequestConfirm ChangeEmailAddressConfirm { get; set; }

    public MediatR_ChangeEmailAddressRequestConfirmationCommand
    (
        DTO_ChangeEmailAddressRequestConfirm changeEmailAddressConfirm
    )
    {
        ChangeEmailAddressConfirm = changeEmailAddressConfirm;
    }
}