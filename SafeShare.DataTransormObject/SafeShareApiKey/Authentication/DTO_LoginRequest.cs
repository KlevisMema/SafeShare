using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.SafeShareApiKey.Authentication;

public class DTO_LoginRequest
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}