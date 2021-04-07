using Skywalker.Application.Dtos.Contracts;
using Skywalker.Application.Services;
using Skywalker.Domain.Entities;
using Skywalker.Domain.Repositories;
using System;

namespace Simple.Application
{
    public class SimpleApplicationService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput> : CrudAppService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        where TEntity : class, IEntity<TKey>
        where TGetOutputDto : IEntityDto<TKey>
        where TGetListOutputDto : IEntityDto<TKey>
    {
        public SimpleApplicationService(IServiceProvider serviceProvider, IRepository<TEntity, TKey> entities) : base(serviceProvider, entities)
        {
        }
    }
}
