/*
 * Represents a base abstract class for various entities in the application.
 * This base class provides common properties for tracking creation, modification, and deletion timestamps.
*/

using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataAccessLayer.BaseModels;

public abstract class Base
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