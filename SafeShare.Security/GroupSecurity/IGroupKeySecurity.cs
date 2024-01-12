namespace SafeShare.Security.GroupSecurity;

public interface IGroupKeySecurity
{
    string
    ProtectCryptoKey
    (
        Guid groupId
    );

    Task<byte[]?>
    DeriveUserKey
    (
        int iterations,
        int outputLength,
        string userId,
        Guid groupId
    );
}