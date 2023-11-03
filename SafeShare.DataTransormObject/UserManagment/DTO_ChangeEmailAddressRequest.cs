using SafeShare.DataTransormObject.Validators;
using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.UserManagment;
public class DTO_ChangeEmailAddressRequest
{
    [Required, EmailAddress]
    public string CurrentEmailAddress { get; set; } = string.Empty;

    [Required, EmailAddress, IsNotEqualTo("CurrentEmailAddress")]
    public string NewEmailAddress { get; set; } = string.Empty;

    [Required, EmailAddress, IsNotEqualTo("CurrentEmailAddress")]
    public string ConfirmNewEmailAddress { get; set; } = string.Empty;

}