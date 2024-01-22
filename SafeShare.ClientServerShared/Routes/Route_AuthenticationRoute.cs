/* 
 * Contains route definitions for various aspects of the SafeShare client-server communication.
 */

namespace SafeShare.ClientServerShared.Routes;

/// <summary>
/// Routes for authentication processes.
/// </summary>
public static class Route_AuthenticationRoute
{
    public const string Login = "Login";
    public const string Register = "Register";
    public const string JwtToken = "GetJwtToken";
    public const string LogOut = "LogOut/{userId}";
    public const string RefreshToken = "RefreshToken";
    public const string ConfirmLogin = "ConfirmLogin/{userId}";
    public const string ConfirmRegistration = "ConfirmRegistration";
    public const string ReConfirmRegistrationRequest = "ReConfirmRegistrationRequest";
}