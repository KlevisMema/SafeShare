/* 
 * Defines a MediatR query handler for retrieving user details.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.MediatR.Dependencies;
using SafeShare.UserManagment.Interfaces;
using SafeShare.DataTransormObject.UserManagment;
using SafeShare.MediatR.Actions.Queries.UserManagment;

namespace SafeShare.MediatR.Handlers.QueriesHandlers.UserManagment;

/// <summary>
/// Represents a MediatR handler that processes queries to get user details.
/// </summary>
public class MediatR_GetUserQueryHandler : MediatR_GenericHandler<IAccountManagment>, IRequestHandler<MediatR_GetUserQuery, ObjectResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_GetUserQueryHandler"/> class with the specified account management service.
    /// </summary>
    /// <param name="accountManagment">The account management service.</param>
    public MediatR_GetUserQueryHandler
    (
        IAccountManagment accountManagment
    )
    : base
    (
        accountManagment
    )
    { }

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