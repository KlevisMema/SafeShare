using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.GroupManagment.GroupInvitations;

public class DTO_SendInvitationRequest
{
    [Required]
    public Guid InvitingUserId { get; set; }
    [Required]
    public Guid InvitedUserId { get; set; }
    [Required]
    public Guid GroupId { get; set; }

    [Required]
    public string InvitaitonMessage { get; set; } = string.Empty;
}