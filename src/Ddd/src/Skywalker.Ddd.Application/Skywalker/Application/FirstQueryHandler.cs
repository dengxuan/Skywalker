using Skywalker.Application.Dtos.Contracts;
using Skywalker.Ddd.Application.Abstractions;
using Skywalker.Ddd.ObjectMapping;
using Skywalker.Domain.Entities;
using Skywalker.Domain.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.Application
{
    public class FirstQueryHandler<TInputDto, TOutputDto, TEntity> : FirstQueryHandler<TInputDto, TOutputDto, TEntity, Guid>, IApplicationHandler<TInputDto, TOutputDto>
        where TInputDto : IEntityDto<Guid>
        where TOutputDto : IEntityDto<Guid>
        where TEntity : class, IEntity<Guid>
    {
        public FirstQueryHandler(IDomainService<TEntity> domainService, IObjectMapper objectMapper) : base(domainService, objectMapper)
        {
        }
    }

    public class FirstQueryHandler<TInputDto, TOutputDto, TEntity, TKey> : IApplicationHandler<TInputDto, TOutputDto>
        where TInputDto : IEntityDto<TKey>
        where TOutputDto : IEntityDto<TKey>
        where TEntity : class, IEntity<TKey>
    {
        private readonly IObjectMapper _objectMapper;
        private readonly IDomainService<TEntity, TKey> _domainService;

        public FirstQueryHandler(IDomainService<TEntity, TKey> domainService, IObjectMapper objectMapper)
        {
            _domainService = domainService;
            _objectMapper = objectMapper;
        }

        public async Task<TOutputDto?> HandleAsync(TInputDto query, CancellationToken cancellationToken = default)
        {
            TEntity? entity = await _domainService.FindAsync(query.Id);
            return _objectMapper.Map<TEntity?, TOutputDto>(entity);
        }
    }
}
