using Microsoft.Extensions.DependencyInjection;
using Skywalker.Application.Dtos;
using Skywalker.Application.Services.Contracts;
using Skywalker.Domain.Entities;
using Skywalker.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace Skywalker.Application.Services
{
    public abstract class BasicCrudAppService<TEntity, TEntityDto, TKey>
        : BasicCrudAppService<TEntity, TEntityDto, TKey, PagedAndSortedResultRequestDto>
        where TEntity : class, IEntity
    {
        protected BasicCrudAppService(IRepository<TEntity> repository)
            : base(repository)
        {

        }
    }

    public abstract class BasicCrudAppService<TEntity, TEntityDto, TKey, TGetListInput>
        : BasicCrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TEntityDto, TEntityDto>
        where TEntity : class, IEntity
    {
        protected BasicCrudAppService(IRepository<TEntity> repository)
            : base(repository)
        {

        }
    }

    public abstract class BasicCrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TCreateInput>
        : BasicCrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TCreateInput, TCreateInput>
        where TEntity : class, IEntity
    {
        protected BasicCrudAppService(IRepository<TEntity> repository)
            : base(repository)
        {

        }
    }

    public abstract class BasicCrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        : BasicCrudAppService<TEntity, TEntityDto, TEntityDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        where TEntity : class, IEntity
    {
        protected BasicCrudAppService(IRepository<TEntity> repository)
            : base(repository)
        {

        }

        protected override TEntityDto MapToGetListOutputDto(TEntity entity)
        {
            return MapToGetOutputDto(entity);
        }
    }

    public abstract class BasicCrudAppService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        : BasicReadOnlyAppService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput>,
            ICrudAppService<TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        where TEntity : class, IEntity
    {
        protected new IRepository<TEntity> Repository { get; }

        protected virtual string CreatePolicyName { get; set; }

        protected virtual string UpdatePolicyName { get; set; }

        protected virtual string DeletePolicyName { get; set; }

        protected BasicCrudAppService(IRepository<TEntity> repository)
            : base(repository)
        {
            Repository = repository;
        }

        public virtual async Task<TGetOutputDto> CreateAsync(TCreateInput input)
        {
            await CheckCreatePolicyAsync();

            var entity = MapToEntity(input);

            await Repository.InsertAsync(entity);

            return MapToGetOutputDto(entity);
        }

        public virtual async Task<TGetOutputDto> UpdateAsync(TKey id, TUpdateInput input)
        {
            await CheckUpdatePolicyAsync();

            var entity = await GetEntityByIdAsync(id);
            //TODO: Check if input has id different than given id and normalize if it's default value, throw ex otherwise
            MapToEntity(input, entity);
            await Repository.UpdateAsync(entity);

            return MapToGetOutputDto(entity);
        }

        public virtual async Task DeleteAsync(TKey id)
        {
            await CheckDeletePolicyAsync();

            await DeleteByIdAsync(id);
        }

        protected abstract Task DeleteByIdAsync(TKey id);

        protected virtual async Task CheckCreatePolicyAsync()
        {
            await CheckPolicy(CreatePolicyName);
        }

        protected virtual async Task CheckUpdatePolicyAsync()
        {
            await CheckPolicy(UpdatePolicyName);
        }

        protected virtual async Task CheckDeletePolicyAsync()
        {
            await CheckPolicy(DeletePolicyName);
        }

        /// <summary>
        /// Maps <see cref="TCreateInput"/> to <see cref="TEntity"/> to create a new entity.
        /// It uses <see cref="IObjectMapper"/> by default.
        /// It can be overriden for custom mapping.
        /// </summary>
        protected virtual TEntity MapToEntity(TCreateInput createInput)
        {
            var entity = ObjectMapper.Map<TCreateInput, TEntity>(createInput);
            SetIdForGuids(entity);
            return entity;
        }

        /// <summary>
        /// Sets Id value for the entity if <see cref="TKey"/> is <see cref="Guid"/>.
        /// It's used while creating a new entity.
        /// </summary>
        protected virtual void SetIdForGuids(TEntity entity)
        {
            if (entity is IEntity<Guid> entityWithGuidId && entityWithGuidId.Id == Guid.Empty)
            {
                EntityHelper.TrySetId(
                    entityWithGuidId,
                    () => GuidGenerator.Create(),
                    true
                );
            }
        }

        /// <summary>
        /// Maps <see cref="TUpdateInput"/> to <see cref="TEntity"/> to update the entity.
        /// It uses <see cref="IObjectMapper"/> by default.
        /// It can be overriden for custom mapping.
        /// </summary>
        protected virtual void MapToEntity(TUpdateInput updateInput, TEntity entity)
        {
            ObjectMapper.Map(updateInput, entity);
        }
    }
}