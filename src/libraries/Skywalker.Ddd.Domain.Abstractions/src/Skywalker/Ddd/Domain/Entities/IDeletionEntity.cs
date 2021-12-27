using System;

namespace Skywalker.Ddd.Domain.Entities
{
    /// <summary>
    /// This interface can be implemented to store deletion information (who delete and when deleted).
    /// </summary>
    public interface IDeletionEntity
    {
        /// <summary>
        /// Id of the deleter user.
        /// </summary>
        Guid? DeleterId { get; set; }

        /// <summary>
        /// Deletion time.
        /// </summary>
        DateTime? DeletionTime { get; set; }
    }

    /// <summary>
    /// Extends <see cref="IDeletionEntity"/> to add user navigation propery.
    /// </summary>
    /// <typeparam name="TUser">Type of the user</typeparam>
    public interface IDeletionEntity<TUser> : IDeletionEntity
    {
        /// <summary>
        /// Reference to the deleter user.
        /// </summary>
        TUser Deleter { get; set; }
    }
}