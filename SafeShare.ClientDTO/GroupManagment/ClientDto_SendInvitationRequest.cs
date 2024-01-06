using SafeShare.ClientDTO.Validators;
using System.ComponentModel.DataAnnotations;

namespace SafeShare.ClientDTO.GroupManagment;

public class ClientDto_SendInvitationRequest
{
    public Guid InvitingUserId { get; set; }
    [Required]
    public Guid InvitedUserId { get; set; }
    [Required]
    public Guid GroupId { get; set; }

    [NoXss]
    [Required]
    public string InvitaitonMessage { get; set; } = string.Empty;
}