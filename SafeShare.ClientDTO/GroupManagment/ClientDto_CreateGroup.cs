using SafeShare.ClientDTO.Validators;
using System.ComponentModel.DataAnnotations;

namespace SafeShare.ClientDTO.GroupManagment;

public class ClientDto_CreateGroup
{
    [NoXss]
    [Required(ErrorMessage = "Group name is required"), StringLength(100)]
    public string GroupName { get; set; } = string.Empty;

    [NoXss]
    [Required(ErrorMessage = "Group description is required"), StringLength(200)]
    public string GroupDescription { get; set; } = string.Empty;
}