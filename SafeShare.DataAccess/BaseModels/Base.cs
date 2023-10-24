using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataAccessLayer.BaseModels;

public abstract class Base : BaseId
{
    /// <summary>
    /// Gets or sets the date created of the entity
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date modified of the entity
    /// </summary>
    [Required]
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// Gets or sets the date deleted of the entity
    /// </summary>
    [Required]
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Gets or sets the is deleted of the entity
    /// </summary>
    [Required]
    public bool IsDeleted { get; set; }
}