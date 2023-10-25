using SafeShare.DataTransormObject.Authentication;
using SafeShare.Utilities.Responses;

namespace SafeShare.Authentication.Interfaces
{
    public interface IAUTH_Register
    {
        Task<Util_GenericResponse<bool>> RegisterUser(DTO_Register registerDto);
    }
}