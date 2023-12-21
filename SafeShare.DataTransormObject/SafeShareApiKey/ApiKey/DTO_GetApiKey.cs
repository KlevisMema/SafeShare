using SafeShare.Utilities.SafeShareApi.Enums;
using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.SafeShareApiKey.ApiKey;

public class DTO_GetApiKey
{
    public Guid ApiKeyId { get; set; }
    public string KeyHash { get; set; } = string.Empty;
    public WorkEnvironment Environment { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ExpiresOn { get; set; }
    public bool IsActive { get; set; }
    public Guid ApiClientId { get; set; }
}