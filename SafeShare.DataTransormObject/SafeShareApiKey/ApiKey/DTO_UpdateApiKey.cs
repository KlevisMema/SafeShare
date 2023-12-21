﻿using SafeShare.DataTransormObject.SafeShareApiKey.Validators;
using SafeShare.Utilities.SafeShareApi.Enums;

namespace SafeShare.DataTransormObject.SafeShareApiKey.ApiKey;

public class DTO_UpdateApiKey
{
    public bool IsActive { get; set; }

    [EnumValidation]
    public WorkEnvironment Environment { get; set; }
    public DateTime ExpiresOn { get; set; }
}