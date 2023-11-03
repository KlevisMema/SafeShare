using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.UserManagment;

public class DTO_ChangeEmail
{
    [Required, EmailAddress]
    public string NewEmail { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string ConfirmNewEmail { get; set; } = string.Empty;
}