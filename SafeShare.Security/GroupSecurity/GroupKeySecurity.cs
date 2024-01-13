using System.Text;
using System.Collections;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using SafeShare.DataAccessLayer.Context;
using Microsoft.AspNetCore.DataProtection;

namespace SafeShare.Security.GroupSecurity;

public class GroupKeySecurity(IDataProtectionProvider dataProtectionProvider, CryptoKeysDb db) : IGroupKeySecurity
{
    public async Task<byte[]?>
    DeriveUserKey
    (
        int iterations,
        int outputLength,
        string userId,
        Guid groupId,
        Guid tag
    )
    {
        string groupMasterKey = "";

        var group = await db.GroupKeys.FirstOrDefaultAsync(x => x.GroupId == groupId);

        if (group is null)
            return null;

        groupMasterKey = UnprotectCryptoKey(group.CryptoKey, groupId, tag);

        using var pbkdf2 = new Rfc2898DeriveBytes(groupMasterKey, Encoding.UTF8.GetBytes(userId), iterations, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(outputLength);
    }

    private static string
    GenerateGroupMasterKey()
    {
        byte[] groupMasterKey = new byte[32];

#pragma warning disable SYSLIB0023
        using (var rng = new RNGCryptoServiceProvider())
            rng.GetBytes(groupMasterKey);
#pragma warning restore SYSLIB0023

        var key = Encoding.UTF8.GetString(groupMasterKey);

        return key;
    }

    public string
    ProtectCryptoKey
    (
        Guid tag,
        Guid groupId
    )
    {
        var protector = dataProtectionProvider.CreateProtector($"GroupSpecific-{groupId}-{tag}");
        return protector.Protect(GenerateGroupMasterKey());
    }

    private string
    UnprotectCryptoKey
    (
        string cryptoKey,
        Guid groupId,
        Guid tag
    )
    {
        var protector = dataProtectionProvider.CreateProtector($"GroupSpecific-{groupId}-{tag}");
        return protector.Unprotect(cryptoKey);
    }
}