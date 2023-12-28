using SafeShare.ClientDTO.Enums;

namespace SafeShare.ClientDTO.GroupManagment;

public class ClientDto_SentInvitations
{
    public string User { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public DateTime InvitationTimeSend { get; set; }
    public InvitationStatus InvitationStatus { get; set; }
    public Guid InvitationId { get; set; }
    public Guid InvitedUserId { get; set; }
    public Guid GroupId { get; set; }
}