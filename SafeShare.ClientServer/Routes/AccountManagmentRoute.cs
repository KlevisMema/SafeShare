namespace SafeShare.ClientServer.Routes;

public static class AccountManagmentRoute
{
    public const string GetUser = "GetUser/{userId}";
    public const string ResetPassword = "ResetPassword";
    public const string ForgotPassword = "ForgotPassword";
    public const string UpdateUser = "UpdateUser/{userId}";
    public const string ChangePassword = "ChangePassword/{userId}";
    public const string DeactivateAccount = "DeactivateAccount/{userId}";
    public const string ActivateAccountRequest = "ActivateAccountRequest";
    public const string RequestChangeEmail = "RequestChangeEmail/{userId}";
    public const string ConfirmChangeEmailRequest = "ConfirmChangeEmailRequest/{userId}";
    public const string ActivateAccountRequestConfirmation = "ActivateAccountRequestConfirmation";
}