using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.UserManagment;

namespace SafeShare.MediatR.Actions.Commands.UserManagment;

public class MediatR_ActivateAccountConfirmationRequestCommand : IRequest<ObjectResult>
{
    public DTO_ActivateAccountConfirmation DTO_ActivateAccount { get; set; }

    public MediatR_ActivateAccountConfirmationRequestCommand
    (
        DTO_ActivateAccountConfirmation dTO_ActivateAccount
    )
    {
        DTO_ActivateAccount = dTO_ActivateAccount;
    }
}