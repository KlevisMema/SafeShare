using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.Authentication;

public class DTO_ConfirmRegistration
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
}