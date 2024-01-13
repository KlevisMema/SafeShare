using SafeShare.ClientDTO.Enums;

namespace SafeShare.ClientDTO.AccountManagment;

public class ClientDto_UserInfo
{
    public string UserID { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int Age { get; set; }
    public DateTime Birthday { get; set; }
    public Gender Gender { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public DateTime? LastLogIn { get; set; }
    public DateTime? LastLogOut { get; set; }
    public bool RequireOTPDuringLogin { get; set; }
    public byte[]? ProfilePicture { get; set; }
    public ClientDto_Token? UserToken { get; set; }
}