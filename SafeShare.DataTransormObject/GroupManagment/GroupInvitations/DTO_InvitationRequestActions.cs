using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.GroupManagment.GroupInvitations;

public class DTO_InvitationRequestActions
{
    [Required]
    public Guid InvitingUserId { get; set; }
    [Required]
    public Guid GroupId { get; set; }
    [Required]
    public Guid InvitationId { get; set; }
    [Required]
    public Guid InvitedUserId { get; set; }
}