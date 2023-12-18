using Microsoft.AspNetCore.DataProtection;
using SafeShare.Security.User.Implementation;

namespace SafeShare.Security.User.Interfaces;

public class Security_UserDataProtectionService(IDataProtectionProvider dataProtectionProvider) : ISecurity_UserDataProtectionService
{
    private readonly IDataProtectionProvider _dataProtectionProvider = dataProtectionProvider;

    public string
    Protect
    (
        string data,
        string userId
    )
    {
        var protector = _dataProtectionProvider.CreateProtector($"UserSpecific-{userId}");
        return protector.Protect(data);
    }

    public string
    Unprotect
    (
        string protectedData,
        string userId
    )
    {
        var protector = _dataProtectionProvider.CreateProtector($"UserSpecific-{userId}");
        return protector.Unprotect(protectedData);
    }
}