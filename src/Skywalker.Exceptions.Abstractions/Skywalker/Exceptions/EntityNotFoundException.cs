namespace Skywalker.Exceptions;

[Serializable]
public class EntityNotFoundException : SkywalkerException, IHasErrorCode
{
    public string? Code => "Skywalker:EntityNotFound";
    public Type? EntityType { get; }
    public object? EntityId { get; }

    public EntityNotFoundException() { }

    public EntityNotFoundException(Type entityType) : this(entityType, null, null) { }

    public EntityNotFoundException(Type entityType, object? entityId) : this(entityType, entityId, null) { }

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

    public EntityNotFoundException(string message) : base(message) { }

    public EntityNotFoundException(string message, Exception? innerException) : base(message, innerException) { }
}
