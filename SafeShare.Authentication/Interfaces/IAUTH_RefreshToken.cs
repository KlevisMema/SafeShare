using SafeShare.Utilities.Responses;
using SafeShare.DataTransormObject.Security;

namespace SafeShare.Authentication.Interfaces;

public interface IAUTH_RefreshToken
{
    Task<Util_GenericResponse<DTO_Token>> 
    RefreshToken
    (
        DTO_ValidateToken validateTokenDto
    );
}