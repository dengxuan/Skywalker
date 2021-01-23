using Microsoft.Extensions.DependencyInjection;
using Skywalker.Application.Dtos;
using Skywalker.Application.Dtos.Contracts;
using Skywalker.Application.Services.Contracts;
using Skywalker.Domain.Entities;
using Skywalker.Domain.Repositories;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Skywalker.Application.Services
{
    public abstract class BasicReadOnlyAppService<TEntity, TEntityDto, TKey>
        : BasicReadOnlyAppService<TEntity, TEntityDto, TEntityDto, TKey, PagedAndSortedResultRequestDto>
        where TEntity : class, IEntity
    {
        protected BasicReadOnlyAppService(ILazyLoader lazyLoader, IReadOnlyRepository<TEntity> repository)
            : base(lazyLoader, repository)
        {

        }
    }

    public abstract class BasicReadOnlyAppService<TEntity, TEntityDto, TKey, TGetListInput>
        : BasicReadOnlyAppService<TEntity, TEntityDto, TEntityDto, TKey, TGetListInput>
        where TEntity : class, IEntity
    {
        protected BasicReadOnlyAppService(ILazyLoader lazyLoader, IReadOnlyRepository<TEntity> repository)
            : base(lazyLoader, repository)
        {

        }
    }

    public abstract class BasicReadOnlyAppService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput>
        : ApplicationService
        , IReadOnlyAppService<TGetOutputDto, TGetListOutputDto, TKey, TGetListInput>
        where TEntity : class, IEntity
    {
        protected IReadOnlyRepository<TEntity> Repository { get; }

        protected virtual string GetPolicyName { get; set; }

        protected virtual string GetListPolicyName { get; set; }

        protected BasicReadOnlyAppService(ILazyLoader lazyLoader, IReadOnlyRepository<TEntity> repository) : base(lazyLoader)
        {
            Repository = repository;
        }

        public virtual async Task<TGetOutputDto> GetAsync(TKey id)
        {
            await CheckGetPolicyAsync();

            var entity = await GetEntityByIdAsync(id);
            return MapToGetOutputDto(entity);
        }

        public virtual async Task<PagedResultDto<TGetListOutputDto>> GetListAsync(TGetListInput input)
        {
            await CheckGetListPolicyAsync();

            var query = CreateFilteredQuery(input);

            var totalCount = await AsyncExecuter.CountAsync(query);

            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var entities = await AsyncExecuter.ToListAsync(query);

            return new PagedResultDto<TGetListOutputDto>(
                totalCount,
                entities.Select(MapToGetListOutputDto).ToList()
            );
        }

        protected abstract Task<TEntity> GetEntityByIdAsync(TKey id);

        protected virtual async Task CheckGetPolicyAsync()
        {
            await CheckPolicy(GetPolicyName);
        }

        protected virtual async Task CheckGetListPolicyAsync()
        {
            await CheckPolicy(GetListPolicyName);
        }

        /// <summary>
        /// Should apply sorting if needed.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="input">The input.</param>
        protected virtual IQueryable<TEntity> ApplySorting(IQueryable<TEntity> query, TGetListInput input)
        {
            //Try to sort query if available
            if (input is ISortedResultRequest sortInput)
            {
                if (!sortInput.Sorting.IsNullOrWhiteSpace())
                {
                    return query.OrderBy(sortInput.Sorting);
                }
            }

            //IQueryable.Task requires sorting, so we should sort if Take will be used.
            if (input is ILimitedResultRequest)
            {
                return ApplyDefaultSorting(query);
            }

            //No sorting
            return query;
        }

        /// <summary>
        /// Applies sorting if no sorting specified but a limited result requested.
        /// </summary>
        /// <param name="query">The query.</param>
        protected virtual IQueryable<TEntity> ApplyDefaultSorting(IQueryable<TEntity> query)
        {
            // Todo: 添加默认排序
            if (typeof(TEntity).IsAssignableTo<IEntity<Guid>>())
            {
                return query.OrderByDescending(e => ((IEntity<Guid>)e).Id);
            }

            throw new SkywalkerException("No sorting specified but this query requires sorting. Override the ApplyDefaultSorting method for your application service derived from AbstractKeyReadOnlyAppService!");
        }

        /// <summary>
        /// Should apply paging if needed.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="input">The input.</param>
        protected virtual IQueryable<TEntity> ApplyPaging(IQueryable<TEntity> query, TGetListInput input)
        {
            //Try to use paging if available
            if (input is IPagedResultRequest pagedInput)
            {
                return query.Page(pagedInput);
            }

            //Try to limit query result if available
            if (input is ILimitedResultRequest limitedInput)
            {
                return query.Take(limitedInput.MaxResultCount);
            }

            //No paging
            return query;
        }

        /// <summary>
        /// This method should create <see cref="IQueryable{TEntity}"/> based on given input.
        /// It should filter query if needed, but should not do sorting or paging.
        /// Sorting should be done in <see cref="ApplySorting"/> and paging should be done in <see cref="ApplyPaging"/>
        /// methods.
        /// </summary>
        /// <param name="input">The input.</param>
        protected virtual IQueryable<TEntity> CreateFilteredQuery(TGetListInput input)
        {
            return Repository;
        }

        /// <summary>
        /// Maps <see cref="TEntity"/> to <see cref="TGetOutputDto"/>.
        /// It uses <see cref="IObjectMapper"/> by default.
        /// It can be overriden for custom mapping.
        /// </summary>
        protected virtual TGetOutputDto MapToGetOutputDto(TEntity entity)
        {
            return ObjectMapper.Map<TEntity, TGetOutputDto>(entity);
        }

        /// <summary>
        /// Maps <see cref="TEntity"/> to <see cref="TGetListOutputDto"/>.
        /// It uses <see cref="IObjectMapper"/> by default.
        /// It can be overriden for custom mapping.
        /// </summary>
        protected virtual TGetListOutputDto MapToGetListOutputDto(TEntity entity)
        {
            return ObjectMapper.Map<TEntity, TGetListOutputDto>(entity);
        }
    }
}
