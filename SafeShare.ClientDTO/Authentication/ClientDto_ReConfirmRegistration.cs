using System.ComponentModel.DataAnnotations;

namespace SafeShare.ClientDTO.Authentication;

public class ClientDto_ReConfirmRegistration
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}