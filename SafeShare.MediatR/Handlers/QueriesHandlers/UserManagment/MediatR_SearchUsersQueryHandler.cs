using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;
using SafeShare.DataTransormObject.UserManagment;
using SafeShare.MediatR.Actions.Queries.UserManagment;

namespace SafeShare.MediatR.Handlers.QueriesHandlers.UserManagment;

public class MediatR_SearchUsersQueryHandler
(
    IAccountManagment service
) : MediatR_GenericHandler<IAccountManagment>(service), 
    IRequestHandler<MediatR_SearchUsersQuery, ObjectResult>
{
    public async Task<ObjectResult>
    Handle
    (
        MediatR_SearchUsersQuery request,
        CancellationToken cancellationToken
    )
    {
        var getUserSearchedResult = await _service.SearchUserByUserName(request.UserName, request.UserId.ToString());

        return Util_GenericControllerResponse<DTO_UserSearched>.ControllerResponseList(getUserSearchedResult);
    }
}