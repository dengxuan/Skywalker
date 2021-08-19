using Skywalker.Application.Dtos.Contracts;
using System;

namespace Skywalker.Application.Dtos
{
    [Serializable]
    public class EntityDto : IEntityDto //TODO: Consider to delete this class
    {
        public override string ToString()
        {
            return $"[DTO: {GetType().Name}]";
        }
    }

    [Serializable]
    public class EntityDto<TKey> : EntityDto, IEntityDto<TKey>
    {
        /// <summary>
        /// Id of the entity.
        /// </summary>
        public TKey Id { get; set; }

        /// <summary>
        /// Just for ObjectMapping and subclasses
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected EntityDto() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public EntityDto(TKey id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return $"[DTO: {GetType().Name}] Id = {Id}";
        }
    }
}