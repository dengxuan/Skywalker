using Microsoft.Extensions.DependencyInjection;
using Skywalker.Application.Dtos.Contracts;
using Skywalker.Ddd.UnitOfWork.Abstractions;

namespace Skywalker.Application.Abstractions;

public interface IExecuteQueryHandler<TOutputDto> : IUnitOfWorkEnabled, IScopedDependency where TOutputDto : IEntityDto
{
    Task<TOutputDto?> HandleAsync(CancellationToken cancellationToken = default);
}

public interface IExecuteQueryHandler<in TInputDto, TOutputDto> : IUnitOfWorkEnabled, IScopedDependency where TInputDto : IEntityDto where TOutputDto : IEntityDto
{
    Task<TOutputDto?> HandleAsync(TInputDto inputDto, CancellationToken cancellationToken = default);
}