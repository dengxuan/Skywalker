using Skywalker.Application.Dtos;
using Skywalker.Application.Dtos.Contracts;
using Skywalker.Domain.Entities;
using Skywalker.Domain.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Skywalker.Application.Services
{
    public abstract class ReadOnlyAppService<TEntity, TEntityDto, TKey>
        : ReadOnlyAppService<TEntity, TEntityDto, TEntityDto, TKey, PagedAndSortedResultRequestDto>
        where TEntity : class, IEntity<TKey>
        where TEntityDto : IEntityDto<TKey>
    {
        protected ReadOnlyAppService(IServiceProvider serviceProvider, IReadOnlyRepository<TEntity, TKey> repository)
            : base(serviceProvider, repository)
        {

        }
    }

    public abstract class ReadOnlyAppService<TEntity, TEntityDto, TKey, TGetListInput>
        : ReadOnlyAppService<TEntity, TEntityDto, TEntityDto, TKey, TGetListInput>
        where TEntity : class, IEntity<TKey>
        where TEntityDto : IEntityDto<TKey>
    {
        protected ReadOnlyAppService(IServiceProvider serviceProvider, IReadOnlyRepository<TEntity, TKey> repository)
            : base(serviceProvider, repository)
        {

        }
    }

    public abstract class ReadOnlyAppService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput>
        : BasicReadOnlyAppService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput>
        where TEntity : class, IEntity<TKey>
        where TGetOutputDto : IEntityDto<TKey>
        where TGetListOutputDto : IEntityDto<TKey>
    {
        protected new IReadOnlyRepository<TEntity, TKey> Repository { get; }

        protected ReadOnlyAppService(IServiceProvider serviceProvider, IReadOnlyRepository<TEntity, TKey> repository)
        : base(serviceProvider, repository)
        {
            Repository = repository;
        }

        protected override async Task<TEntity> GetEntityByIdAsync(TKey id)
        {
            return await Repository.GetAsync(id);
        }

        protected override IQueryable<TEntity> ApplyDefaultSorting(IQueryable<TEntity> query)
        {
            // Todo: 默认按照创建时间排序
            return query.OrderByDescending(e => e.Id);
        }
    }
}
