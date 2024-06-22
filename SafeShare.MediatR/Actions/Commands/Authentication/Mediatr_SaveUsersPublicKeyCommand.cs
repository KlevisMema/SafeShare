using MediatR;
using SafeShare.Utilities.SafeShareApi.Responses;

namespace SafeShare.MediatR.Actions.Commands.Authentication;

public class Mediatr_SaveUsersPublicKeyCommand
(
    Guid userId,
    string publicKey
) : IRequest<Util_GenericResponse<string>>
{
    public Guid UserId { get; set; } = userId;
    public string PublicKey { get; set; } = publicKey;
}