/* 
 * Contains route definitions for various aspects of the SafeShare client-server communication.
 */

namespace SafeShare.ClientServerShared.Routes;

/// <summary>
/// Routes related to account management functionalities.
/// </summary>
public static class Route_AccountManagmentRoute
{
    public const string GetUser = "GetUser/{userId}";
    public const string ResetPassword = "ResetPassword";
    public const string ForgotPassword = "ForgotPassword";
    public const string UpdateUser = "UpdateUser/{userId}";
    public const string ChangePassword = "ChangePassword/{userId}";
    public const string DeactivateAccount = "DeactivateAccount/{userId}";
    public const string ActivateAccountRequest = "ActivateAccountRequest";
    public const string RequestChangeEmail = "RequestChangeEmail/{userId}";
    public const string SearchUserByUserName = "SearchUserByUserName/{userId}";
    public const string ConfirmChangeEmailRequest = "ConfirmChangeEmailRequest/{userId}";
    public const string ActivateAccountRequestConfirmation = "ActivateAccountRequestConfirmation";
}