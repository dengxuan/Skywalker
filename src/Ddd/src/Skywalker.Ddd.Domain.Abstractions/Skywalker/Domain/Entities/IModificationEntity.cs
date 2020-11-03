using System;

namespace Skywalker.Domain.Entities
{
    /// <summary>
    /// This interface can be implemented to store modification information (who and when modified lastly).
    /// </summary>
    public interface IModificationEntity
    {
        /// <summary>
        /// Last modifier user for this entity.
        /// </summary>
        Guid? LastModifierId { get; set; }

        /// <summary>
        /// The last modified time for this entity.
        /// </summary>
        DateTime? LastModificationTime { get; set; }
    }

    /// <summary>
    /// Adds navigation properties to <see cref="IModificationEntity"/> interface for a user.
    /// </summary>
    /// <typeparam name="TUser">Type of the user</typeparam>
    public interface IModificationAuditedObject<TUser> : IModificationEntity
    {
        /// <summary>
        /// Reference to the last modifier user of this entity.
        /// </summary>
        TUser LastModifier { get; set; }
    }
}