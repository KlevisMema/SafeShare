using SafeShare.Utilities.Enums;
using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.UserManagment;
public class DTO_UserInfo
{
    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public DateTime Birthday { get; set; }

    [Required]
    public Gender Gender { get; set; }

    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required, DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; } = string.Empty;
}