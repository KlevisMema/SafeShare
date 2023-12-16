using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.Authentication;

public class DTO_ReConfirmRegistration
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}