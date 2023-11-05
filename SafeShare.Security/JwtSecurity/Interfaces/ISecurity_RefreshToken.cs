using SafeShare.Utilities.Responses;
using SafeShare.DataTransormObject.Security;

namespace SafeShare.Security.JwtSecurity.Interfaces;

public interface ISecurity_RefreshToken
{
    Task<Util_GenericResponse<DTO_Token>>
    RefreshToken
    (
        DTO_ValidateToken validateTokenDto
    );
}