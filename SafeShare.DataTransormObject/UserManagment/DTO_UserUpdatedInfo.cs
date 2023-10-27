using SafeShare.Utilities.Enums;
using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.UserManagment;

public class DTO_UserUpdatedInfo
{
    public string UserID { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int Age { get; set; }
    public DateTime Birthday { get; set; }
    public Gender Gender { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}