using SafeShare.Utilities.Enums;
using Microsoft.AspNetCore.Identity;

namespace SafeShare.DataAccessLayer.BaseModels;

public abstract class BaseIdentity : IdentityUser
{
    /// <summary>
    /// Gets or sets the full name of the user.
    /// </summary>
    public string FullName { get; set; } = null!; // Non-nullable since a full name is usually required.

    /// <summary>
    /// Gets or sets the birthday of the user.
    /// </summary>
    public DateTime Birthday { get; set; }

    /// <summary>
    /// Gets or sets the gender of the user.  
    /// </summary>
    public Gender Gender { get; set; }

    /// <summary>
    /// Gets or sets the is deleted of the user
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the date created of the user
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date modified of the user
    /// </summary>
    public DateTime ModifiedAt { get; set; }

    /// <summary>
    /// Gets or sets the date deleted of the user
    /// </summary>
    public DateTime DeletedAt { get; set; }
}