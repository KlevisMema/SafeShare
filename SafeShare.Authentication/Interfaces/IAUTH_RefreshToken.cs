using SafeShare.DataTransormObject.SafeShareApi.Security;
using SafeShare.Utilities.SafeShareApi.Responses;

namespace SafeShare.Authentication.Interfaces;

public interface IAUTH_RefreshToken
{
    Task<Util_GenericResponse<DTO_Token>> 
    RefreshToken
    (
        DTO_ValidateToken validateTokenDto
    );
}