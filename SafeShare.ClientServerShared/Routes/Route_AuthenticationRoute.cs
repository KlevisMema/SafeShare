namespace SafeShare.ClientServerShared.Routes;
public static class Route_AuthenticationRoute
{
    public const string Login = "Login";
    public const string LogOut = "LogOut/{userId}";
    public const string Register = "Register";
    public const string ConfirmLogin = "ConfirmLogin";
    public const string RefreshToken = "RefreshToken";
    public const string ConfirmRegistration = "ConfirmRegistration";
    public const string ReConfirmRegistrationRequest = "ReConfirmRegistrationRequest";
}