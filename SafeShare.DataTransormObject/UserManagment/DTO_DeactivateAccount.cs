using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.UserManagment;
public class DTO_DeactivateAccount
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}