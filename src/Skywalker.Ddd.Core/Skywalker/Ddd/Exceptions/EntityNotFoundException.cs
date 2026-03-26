namespace Skywalker.Ddd.Exceptions;

/// <summary>
/// Exception thrown when an entity is not found.
/// </summary>
[Serializable]
public class EntityNotFoundException : SkywalkerException, IHasErrorCode
{
    /// <summary>
    /// Gets the error code.
    /// </summary>
    public string? Code => "Skywalker:EntityNotFound";

    /// <summary>
    /// Gets the type of the entity.
    /// </summary>
    public Type? EntityType { get; }

    /// <summary>
    /// Gets the ID of the entity.
    /// </summary>
    public object? EntityId { get; }

    /// <summary>
    /// Creates a new <see cref="EntityNotFoundException"/> object.
    /// </summary>
    public EntityNotFoundException()
    {
    }

    /// <summary>
    /// Creates a new <see cref="EntityNotFoundException"/> object.
    /// </summary>
    /// <param name="entityType">Type of the entity</param>
    public EntityNotFoundException(Type entityType)
        : this(entityType, null, null)
    {
    }

    /// <summary>
    /// Creates a new <see cref="EntityNotFoundException"/> object.
    /// </summary>
    /// <param name="entityType">Type of the entity</param>
    /// <param name="entityId">ID of the entity</param>
    public EntityNotFoundException(Type entityType, object? entityId)
        : this(entityType, entityId, null)
    {
    }

    /// <summary>
    /// Creates a new <see cref="EntityNotFoundException"/> object.
    /// </summary>
    /// <param name="entityType">Type of the entity</param>
    /// <param name="entityId">ID of the entity</param>
    /// <param name="innerException">Inner exception</param>
    public EntityNotFoundException(Type entityType, object? entityId, Exception? innerException)
        : base(
            entityId == null
                ? $"Entity of type '{entityType.FullName}' was not found."
                : $"Entity of type '{entityType.FullName}' with ID '{entityId}' was not found.",
            innerException)
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    /// <summary>
    /// Creates a new <see cref="EntityNotFoundException"/> object.
    /// </summary>
    /// <param name="message">Exception message</param>
    public EntityNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates a new <see cref="EntityNotFoundException"/> object.
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Inner exception</param>
    public EntityNotFoundException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}

