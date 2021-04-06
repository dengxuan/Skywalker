using Skywalker.Application.Dtos;
using Skywalker.Application.Dtos.Contracts;
using Skywalker.Domain.Entities;
using Skywalker.Domain.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Skywalker.Application.Services
{
    public abstract class CrudAppService<TEntity, TEntityDto, TKey>
        : CrudAppService<TEntity, TEntityDto, TKey, PagedAndSortedResultRequestDto>
        where TEntity : class, IEntity<TKey>
        where TEntityDto : IEntityDto<TKey>
    {
        protected CrudAppService(IServiceProvider serviceProvider, IRepository<TEntity, TKey> repository)
            : base(serviceProvider, repository)
        {

        }
    }

    public abstract class CrudAppService<TEntity, TEntityDto, TKey, TGetListInput>
        : CrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TEntityDto, TEntityDto>
        where TEntity : class, IEntity<TKey>
        where TEntityDto : IEntityDto<TKey>
    {
        protected CrudAppService(IServiceProvider serviceProvider, IRepository<TEntity, TKey> repository)
            : base(serviceProvider, repository)
        {

        }
    }

    public abstract class CrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TCreateInput>
        : CrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TCreateInput, TCreateInput>
        where TEntity : class, IEntity<TKey>
        where TEntityDto : IEntityDto<TKey>
    {
        protected CrudAppService(IServiceProvider serviceProvider, IRepository<TEntity, TKey> repository)
            : base(serviceProvider, repository)
        {

        }
    }

    public abstract class CrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        : CrudAppService<TEntity, TEntityDto, TEntityDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        where TEntity : class, IEntity<TKey>
        where TEntityDto : IEntityDto<TKey>
    {
        protected CrudAppService(IServiceProvider serviceProvider, IRepository<TEntity, TKey> repository)
            : base(serviceProvider, repository)
        {

        }

        protected override TEntityDto MapToGetListOutputDto(TEntity entity)
        {
            return MapToGetOutputDto(entity);
        }
    }

    public abstract class CrudAppService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        : BasicCrudAppService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        where TEntity : class, IEntity<TKey>
        where TGetOutputDto : IEntityDto<TKey>
        where TGetListOutputDto : IEntityDto<TKey>
    {
        protected new IRepository<TEntity, TKey> Repository { get; }

        protected CrudAppService(IServiceProvider serviceProvider, IRepository<TEntity, TKey> repository)
            : base(serviceProvider, repository)
        {
            Repository = repository;
        }

        protected override async Task DeleteByIdAsync(TKey id)
        {
            await Repository.DeleteAsync(id);
        }

        protected override async Task<TEntity> GetEntityByIdAsync(TKey id)
        {
            return await Repository.GetAsync(id);
        }

        protected override void MapToEntity(TUpdateInput updateInput, TEntity entity)
        {
            if (updateInput is IEntityDto<TKey> entityDto)
            {
                entityDto.Id = entity.Id;
            }

            base.MapToEntity(updateInput, entity);
        }

        protected override IQueryable<TEntity> ApplyDefaultSorting(IQueryable<TEntity> query)
        {
            // Todo: 默认按照创建时间排序
            return query.OrderByDescending(e => e.Id);
        }
    }
}
