using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataAccessLayer.Models.SafeShareApi.CryptoModels;

public class GroupKey
{
    [Key]
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public DateTime KeyCreatedTime { get; set; }
    public DateTime? KeyUpdatedTime { get; set; }
    public string CryptoKey { get; set; } = string.Empty;
}