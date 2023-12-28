/* 
 * Contains route definitions for various aspects of the SafeShare client-server communication.
 */

namespace SafeShare.ClientServerShared.Routes;

/// <summary>
/// Base routes for the SafeShare application.
/// </summary>
public static class BaseRoute
{
    public const string Route = "api/[controller]";
    public const string RouteAuthenticationForClient = "api/Authentication/";
    public const string RouteGroupManagmentForClient = "api/GroupManagment/";
    public const string RouteAccountManagmentForClient = "api/AccountManagment/";
    public const string RouteExpenseManagmentForClient = "api/ExpenseManagment/";

    public const string RouteAuthenticationProxy = "api/AuthProxy/";
    public const string RouteAccountManagmentProxy = "api/AccountManagmentProxy/";
    public const string RouteGroupManagmentProxy = "api/GroupManagmentProxy/";
    public const string RouteExpenseManagmentProxy = "api/ExpenseManagmentProxy/";
}