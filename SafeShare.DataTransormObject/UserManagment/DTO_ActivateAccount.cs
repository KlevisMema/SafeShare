using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.UserManagment;
public class DTO_ActivateAccount
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}