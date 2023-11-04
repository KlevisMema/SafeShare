using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.Security;

public class DTO_ValidateToken
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    public string RefreshToken { get; set; } = string.Empty;

    [Required]
    public Guid RefreshTokenId { get; set; }
}