using SafeShare.Utilities.Responses;
using SafeShare.DataAccessLayer.Context;
using SafeShare.DataTransormObject.Authentication;

namespace SafeShare.Authentication.Auth;

public class AUTH_Register
{
    private readonly ApplicationDbContext _db;

    public AUTH_Register
    (
        ApplicationDbContext db
    )
    {
        _db = db;
    }

    //public async Task<Util_GenericResponse<bool>> Register
    //(
    //    DTO_Register register
    //)
    //{
    //    try
    //    {

    //    }
    //    catch (Exception ex)
    //    {
    //    }
    //}
}