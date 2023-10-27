using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.UserManagment;

public class DTO_UserChangePassword
{
    [Required(ErrorMessage = "Old password is required"), DataType(DataType.Password)]
    public string OldPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password is required"), DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm new password is required"), DataType(DataType.Password), Compare("NewPassword")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}