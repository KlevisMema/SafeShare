using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataAccessLayer.Models.SafeShareApiKey;

public class ApiClient : IdentityUser
{
    [Required]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime RegisteredOn { get; set; }

    [Required]
    public bool IsActive { get; set; }

    [Required]
    public string Website { get; set; } = string.Empty;

    [Required]
    public string ContactPerson { get; set; } = string.Empty;

    [Required]
    public string SiteYouDevelopingUrl { get; set; } = string.Empty;

    public virtual ICollection<ApiKey>? ApiKeys { get; set; }
}