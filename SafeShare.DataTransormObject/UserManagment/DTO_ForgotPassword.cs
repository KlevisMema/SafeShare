using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.UserManagment;

public class DTO_ForgotPassword
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}