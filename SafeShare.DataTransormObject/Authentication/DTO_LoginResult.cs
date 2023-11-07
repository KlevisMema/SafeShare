using SafeShare.DataTransormObject.Security;

namespace SafeShare.DataTransormObject.Authentication;

public class DTO_LoginResult
{
    public DTO_Token Token { get; set; } = new();
    public bool RequireOtpDuringLogin { get; set; } = false;
    public string UserId { get; set; } = string.Empty;
}