using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SafeShare.MediatR.Actions.Commands.Authentication;

public class MediatR_LogOutCommand : IRequest<ObjectResult>
{
    public string Id { get; set; }

    public MediatR_LogOutCommand
    (
        string id
    )
    {
        Id = id;
    }
}