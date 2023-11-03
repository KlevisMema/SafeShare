namespace SafeShare.DataTransormObject.Authentication;

public class DTO_LoginResult
{
    public string? Token { get; set; }
    public bool RequireOtpDuringLogin { get; set; } = false;
}