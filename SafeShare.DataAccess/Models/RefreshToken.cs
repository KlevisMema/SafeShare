using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataAccessLayer.Models;
public class RefreshToken
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    [StringLength(64)]
    public string HashedToken { get; set; } = string.Empty;
    public string JwtId { get; set; } = string.Empty;
    public DateTime? CreationDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool Used { get; set; }
    public bool Invaidated { get; set; }
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;
}