using SafeShare.Utilities.SafeShareApi.Enums;
using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataAccessLayer.Models.SafeShareApiKey;

public class ApiKey
{
    [Key]
    public Guid ApiKeyId { get; set; }
    public string KeyHash { get; set; } = string.Empty;
    public WorkEnvironment Environment { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ExpiresOn { get; set; }
    public bool IsActive { get; set; }
    public string ApiClientId { get; set; } = string.Empty;
    public virtual ApiClient ApiClient { get; set; } = null!;
}