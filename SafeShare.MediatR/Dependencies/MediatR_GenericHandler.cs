using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.MediatR.Actions.Commands.UserManagment;
using SafeShare.UserManagment.Interfaces;

namespace SafeShare.MediatR.Dependencies;

public class MediatR_GenericHandler<TService>
{
    protected readonly TService _service;

    public MediatR_GenericHandler
    (
        TService service
    )
    {
        _service = service;
    }
}