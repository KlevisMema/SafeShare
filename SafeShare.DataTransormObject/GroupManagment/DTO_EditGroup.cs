using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.GroupManagment;

public class DTO_EditGroup
{
    [Required(ErrorMessage = "Group name is required"), StringLength(100)]
    public string GroupName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Group description is required"), StringLength(200)]
    public string GroupDescription { get; set; } = string.Empty;
}