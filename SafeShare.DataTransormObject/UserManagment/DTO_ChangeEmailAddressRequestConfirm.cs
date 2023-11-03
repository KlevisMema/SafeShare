using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.UserManagment;
public class DTO_ChangeEmailAddressRequestConfirm
{
    [Required, EmailAddress]
    public string EmailAddress { get; set; } = string.Empty;
    [Required]
    public string Token { get; set; } = string.Empty;
}