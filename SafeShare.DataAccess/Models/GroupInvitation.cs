using SafeShare.Utilities.Enums;
using SafeShare.DataAccessLayer.BaseModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeShare.DataAccessLayer.Models;

public class GroupInvitation : Base
{
    [Key]
    public Guid Id { get; set; }

    [Required, StringLength(200)]
    public string InvitationMessage { get; set; } = null!;

    [Required]
    public InvitationStatus InvitationStatus { get; set; }

    public Guid GroupId { get; set; }
    public virtual Group Group { get; set; } = null!;

    public string InvitedUserId { get; set; }
    public virtual ApplicationUser InvitedUser { get; set; } = null!;

    public string InvitingUserId { get; set; }
    public virtual ApplicationUser InvitingUser { get; set; } = null!;
}