using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.SafeShareApiKey.User;

public class DTO_UpdateApiClient
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(100, ErrorMessage = "Username must be less than 100 characters")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    [Required(ErrorMessage = "Company name is required")]
    [StringLength(200, ErrorMessage = "Company name must be less than 200 characters")]
    public string CompanyName { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description must be less than 500 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Website is required")]
    [Url(ErrorMessage = "Invalid URL format")]
    public string Website { get; set; } = string.Empty;

    [Required(ErrorMessage = "Contact person is required")]
    [StringLength(100, ErrorMessage = "Contact person's name must be less than 100 characters")]
    public string ContactPerson { get; set; } = string.Empty;

    [Required(ErrorMessage = "Development site URL is required")]
    [Url(ErrorMessage = "Invalid URL format")]
    public string SiteYouDevelopingUrl { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number format")]
    public string PhoneNumber { get; set; } = string.Empty;
}