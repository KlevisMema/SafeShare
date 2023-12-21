﻿/* 
 * Contains route definitions for various aspects of the SafeShare client-server communication.
 */

namespace SafeShare.ClientServerShared.Routes;

/// <summary>
/// Routes for group management in the SafeShare application.
/// </summary>
public static class Route_GroupManagmentRoutes
{
    public const string GroupTypes = "GroupTypes/{userId}";
    public const string CreateGroup = "CreateGroup/{userId}";
    public const string EditGroup = "EditGroup/{userId}/{groupId}";
    public const string SendInvitation = "SendInvitation/{userId}";
    public const string DeleteGroup = "DeleteGroup/{userId}/{groupId}";
    public const string AcceptInvitation = "AcceptInvitation/{userId}";
    public const string RejectInvitation = "RejectInvitation/{userId}";
    public const string DeleteInvitation = "DeleteInvitation/{userId}";
    public const string GetGroupDetails = "GetGroupDetails/{userId}/{groupId}";
    public const string GetGroupsInvitations = "GetGroupsInvitations/{userId}";
    public const string GetSentGroupInvitations = "GetSentGroupInvitations/{userId}";
}