using SafeShare.ClientDTO.Enums;
using System.ComponentModel.DataAnnotations;

namespace SafeShare.ClientDTO.AccountManagment;

public class ClientDto_UpdateUser
{
    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;
    [Required]
    public DateTime Birthday { get; set; }
    [Required]
    public Gender Gender { get; set; }
    [Required]
    public string UserName { get; set; } = string.Empty;
    [Required]
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string PhoneNumber { get; set; } = string.Empty;
    public bool Enable2FA { get; set; } = false;
}