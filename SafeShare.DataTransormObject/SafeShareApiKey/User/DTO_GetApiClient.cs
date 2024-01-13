namespace SafeShare.DataTransormObject.SafeShareApiKey.User;

public class DTO_GetApiClient
{
    public string? Email { get; set; } = string.Empty;
    public string CompanyName { get; set; } =  string.Empty;
    public string Description { get; set; } =  string.Empty;
    public DateTime RegisteredOn { get; set; }
    public bool IsActive { get; set; }
    public string Website { get; set; } =  string.Empty;
    public string ContactPerson { get; set; } =  string.Empty;
    public string SiteYouDevelopingUrl { get; set; } =  string.Empty;
}