using Skywalker.Application.Dtos.Contracts;
using Skywalker.Ddd.ObjectMapping;
using Skywalker.Ddd.Queries.Abstractions;
using Skywalker.Domain.Entities;
using Skywalker.Domain.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.Queries
{
    public class SingleOrDefaultQueryHandler<TQuery, TEntity, TOutput> : SingleOrDefaultQueryHandler<TQuery, TEntity, Guid, TOutput>, IQueryHandler<TQuery, TOutput> where TQuery : IEntityDto<Guid> where TEntity : class, IEntity<Guid>
    {
        public SingleOrDefaultQueryHandler(IDomainService<TEntity> domainService, IObjectMapper objectMapper) : base(domainService, objectMapper)
        {
        }
    }

    public class SingleOrDefaultQueryHandler<TQuery, TEntity, TKey, TOutput> : IQueryHandler<TQuery, TOutput> where TQuery : IEntityDto<TKey> where TEntity : class, IEntity<TKey>
    {
        private readonly IObjectMapper _objectMapper;
        private readonly IDomainService<TEntity, TKey> _domainService;

        public SingleOrDefaultQueryHandler(IDomainService<TEntity, TKey> domainService, IObjectMapper objectMapper)
        {
            _domainService = domainService;
            _objectMapper = objectMapper;
        }

        public async Task<TOutput?> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            TEntity entity = await _domainService.GetAsync(query.Id);
            return _objectMapper.Map<TEntity, TOutput>(entity);
        }
    }
}
