/* 
 * Defines a MediatR query handler for retrieving user details.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;
using SafeShare.MediatR.Actions.Queries.UserManagment;
using SafeShare.DataTransormObject.SafeShareApi.UserManagment;
using SafeShare.Utilities.SafeShareApi.Responses;

namespace SafeShare.MediatR.Handlers.QueriesHandlers.UserManagment;

/// <summary>
/// Represents a MediatR handler that processes queries to get user details.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_GetUserQueryHandler"/> class with the specified account management service.
/// </remarks>
/// <param name="accountManagment">The account management service.</param>
public class MediatR_GetUserQueryHandler
(
    IAccountManagment accountManagment
) : MediatR_GenericHandler<IAccountManagment>(accountManagment),
    IRequestHandler<MediatR_GetUserQuery, ObjectResult>
{

    /// <summary>
    /// Processes the provided query to retrieve user details.
    /// </summary>
    /// <param name="request">The user query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result containing user details or errors.</returns>
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