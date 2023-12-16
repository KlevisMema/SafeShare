using SafeShare.ClientDTO.Enums;
using System.ComponentModel.DataAnnotations;

namespace SafeShare.ClientDTO.Authentication;

public class ClientDto_Register
{
    [Required]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Invalid username length")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Invalid fullname length")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public DateTime? Birthday { get; set; }

    [Required]
    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public Gender Gender { get; set; }

    [Required, EmailAddress(ErrorMessage = "Invalid E-mail address")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Invalid password length")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare("Password")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Invalid password length")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public bool TwoFA { get; set; }
}