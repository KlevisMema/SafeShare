using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataAccessLayer.BaseModels;

public abstract class BaseId
{
    /// <summary>
    /// Gets or sets the primary key of the entity
    /// </summary>
    [Key]
    public Guid Id { get; set; }
}