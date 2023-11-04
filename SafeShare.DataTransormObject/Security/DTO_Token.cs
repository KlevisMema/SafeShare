namespace SafeShare.DataTransormObject.Security;

public class DTO_Token
{
    public string? Token { get; set; } = string.Empty;
    public string? RefreshToken { get; set; } = string.Empty;
    public Guid RefreshTokenId { get; set; }
}