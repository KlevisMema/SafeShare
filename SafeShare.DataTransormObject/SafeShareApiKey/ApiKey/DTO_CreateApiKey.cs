using SafeShare.DataTransormObject.SafeShareApiKey.Validators;
using SafeShare.Utilities.SafeShareApi.Enums;

namespace SafeShare.DataTransormObject.SafeShareApiKey.ApiKey;

public class DTO_CreateApiKey
{
    public Guid UserId { get; set; }

    [EnumValidation]
    public WorkEnvironment Environment { get; set; }
    public bool Active { get; set; }
    public DateTime ExpiresOn { get; set; }
}