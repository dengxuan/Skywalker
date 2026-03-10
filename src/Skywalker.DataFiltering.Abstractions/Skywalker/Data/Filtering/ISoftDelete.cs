namespace Skywalker.Data.Filtering;

/// <summary>
/// Interface for entities that support soft delete.
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    /// Gets or sets whether the entity is deleted.
    /// </summary>
    bool IsDeleted { get; set; }
}

