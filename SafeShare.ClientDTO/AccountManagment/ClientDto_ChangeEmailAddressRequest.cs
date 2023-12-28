using SafeShare.ClientDTO.Validators;
using System.ComponentModel.DataAnnotations;

namespace SafeShare.ClientDTO.AccountManagment;

public class ClientDto_ChangeEmailAddressRequest
{
    [Required, EmailAddress]
    public string CurrentEmailAddress { get; set; } = string.Empty;
    [Required, EmailAddress, IsNotEqualTo("CurrentEmailAddress")]
    public string NewEmailAddress { get; set; } = string.Empty;
    [Required, EmailAddress, IsNotEqualTo("CurrentEmailAddress")]
    public string ConfirmNewEmailAddress { get; set; } = string.Empty;
}