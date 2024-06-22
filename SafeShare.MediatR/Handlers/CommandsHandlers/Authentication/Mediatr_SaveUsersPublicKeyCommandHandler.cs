using MediatR;
using SafeShare.Authentication.Interfaces;
using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.MediatR.Actions.Commands.Authentication;
using SafeShare.DataTransormObject.SafeShareApi.Authentication;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.Authentication;

public class Mediatr_SaveUsersPublicKeyCommandHandler
(
    IAUTH_Login loginUser
) : IRequestHandler<Mediatr_SaveUsersPublicKeyCommand, Util_GenericResponse<string>>
{
    public Task<Util_GenericResponse<string>> Handle
    (
        Mediatr_SaveUsersPublicKeyCommand request, 
        CancellationToken cancellationToken
    )
    {
        var result = loginUser.SaveUsersPublicKey(request.UserId, request.PublicKey);

        return result;
    }
}