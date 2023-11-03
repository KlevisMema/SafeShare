using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;

namespace SafeShare.MediatR.Actions.Commands.UserManagment;

public class MediatR_ActivateAccountRequestCommand : IRequest<ObjectResult>
{
    public string Email { get; set; }

    public MediatR_ActivateAccountRequestCommand
    (
        string email
    )
    {
        Email = email;
    }
}