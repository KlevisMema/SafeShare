using Microsoft.AspNetCore.DataProtection;

namespace SafeShare.API.Helpers;

internal class API_Helper_DataProtection(IDataProtectionProvider dataProtectionProvider, string purpose)
{
    private readonly IDataProtector _protector = dataProtectionProvider.CreateProtector(purpose);

    internal string 
    Encrypt
    (
        string input
    )
    {
        return _protector.Protect(input);
    }

    internal string 
    Decrypt
    (
        string encryptedInput
    )
    {
        return _protector.Unprotect(encryptedInput);
    }
}