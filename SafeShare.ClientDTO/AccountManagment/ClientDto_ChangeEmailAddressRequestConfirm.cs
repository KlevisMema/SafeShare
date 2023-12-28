using System.ComponentModel.DataAnnotations;

namespace SafeShare.ClientDTO.AccountManagment;

public class ClientDto_ChangeEmailAddressRequestConfirm
{
    [Required, EmailAddress]
    public string EmailAddress { get; set; } = string.Empty;
    [Required]
    public string Token { get; set; } = string.Empty;
}