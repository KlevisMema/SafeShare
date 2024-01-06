using SafeShare.ClientDTO.Validators;
using System.ComponentModel.DataAnnotations;

namespace SafeShare.ClientDTO.Authentication;

public class ClientDto_ReConfirmRegistration
{
    [NoXss]
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}