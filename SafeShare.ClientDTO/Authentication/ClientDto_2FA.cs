using SafeShare.ClientDTO.Validators;
using System.ComponentModel.DataAnnotations;

namespace SafeShare.ClientDTO.Authentication;

public class ClientDto_2FA
{
    [Required]
    public Guid UserId { get; set; }

    [NoXss]
    [Required]
    public string OTP { get; set; } = string.Empty;
}