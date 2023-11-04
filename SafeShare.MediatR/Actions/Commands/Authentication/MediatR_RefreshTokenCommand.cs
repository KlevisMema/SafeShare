using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.Security;

namespace SafeShare.MediatR.Actions.Commands.Authentication;

public class MediatR_RefreshTokenCommand : IRequest<ObjectResult>
{
    public DTO_ValidateToken DTO_ValidateToken { get; set; }

    public MediatR_RefreshTokenCommand
    (
        DTO_ValidateToken dTO_ValidateToken
    )
    {
        DTO_ValidateToken = dTO_ValidateToken;
    }
}