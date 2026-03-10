namespace Skywalker.Data.Filtering;

/// <summary>
/// Interface for entities that must have a user.
/// </summary>
public interface IMustHaveUser
{
    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    Guid UserId { get; set; }
}

/// <summary>
/// Interface for entities that may have a user.
/// </summary>
public interface IMayHaveUser
{
    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    Guid? UserId { get; set; }
}

