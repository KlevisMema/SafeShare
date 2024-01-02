using System.ComponentModel.DataAnnotations;

namespace SafeShare.ClientDTO.AccountManagment;

public class ClienDto_ActivateAccountRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}