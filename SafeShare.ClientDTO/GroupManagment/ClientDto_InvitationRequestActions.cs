using System.ComponentModel.DataAnnotations;

namespace SafeShare.ClientDTO.GroupManagment;

public class ClientDto_InvitationRequestActions
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