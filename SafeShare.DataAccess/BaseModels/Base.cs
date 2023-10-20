namespace SafeShare.DataAccessLayer.BaseModels;

public abstract class Base : BaseId
{
    /// <summary>
    /// Gets or sets the date created of the entity
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date modified of the entity
    /// </summary>
    public DateTime ModifiedAt { get; set; }

    /// <summary>
    /// Gets or sets the date deleted of the entity
    /// </summary>
    public DateTime DeletedAt { get; set; }
}