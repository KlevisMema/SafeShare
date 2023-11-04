namespace SafeShare.DataTransormObject.GroupManagment.GroupInvitations;
public class DTO_RecivedInvitations
{
    public string InvitationMessage { get; set; } = string.Empty;
    public Guid InvitingUserId { get; set; }
    public string InvitingUserName { get; set; } = string.Empty;
    public Guid GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
}