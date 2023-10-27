using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;
using SafeShare.DataTransormObject.UserManagment;
using SafeShare.MediatR.Actions.Queries.UserManagment;

namespace SafeShare.MediatR.Handlers.QueriesHandlers.UserManagment;

public class MediatR_GetUserQueryHandler : MediatR_GenericHandler<IAccountManagment>, IRequestHandler<MediatR_GetUserQuery, ObjectResult>
{
    public MediatR_GetUserQueryHandler
    (
        IAccountManagment accountManagment
    )
    : base
    (
        accountManagment
    )
    { }

    public async Task<ObjectResult>
    Handle
    (
        MediatR_GetUserQuery request,
        CancellationToken cancellationToken
    )
    {
        var getUserResult = await _service.GetUser(request.Id);

        return Util_GenericControllerResponse<DTO_UserUpdatedInfo>.ControllerResponse(getUserResult);
    }
}