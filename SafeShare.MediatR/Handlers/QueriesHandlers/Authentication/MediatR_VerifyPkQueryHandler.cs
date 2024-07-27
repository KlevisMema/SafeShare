using MediatR;
using SafeShare.Authentication.Interfaces;
using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.MediatR.Actions.Commands.Authentication;
using SafeShare.MediatR.Actions.Queries.Authentication;

namespace SafeShare.MediatR.Handlers.QueriesHandlers.Authentication;
public class MediatR_VerifyPkQueryHandler
(
    IAUTH_Login loginUser
) : IRequestHandler<MediatR_VerifyPkQuery, Util_GenericResponse<bool>>
{
    public Task<Util_GenericResponse<bool>> Handle
    (
        MediatR_VerifyPkQuery request, 
        CancellationToken cancellationToken
    )
    {
        var result = loginUser.VerifyPk(request.UserId, request.PublicKey);

        return result;
    }
}