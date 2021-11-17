using Microsoft.Extensions.DependencyInjection;
using Skywalker.Application.Dtos.Contracts;

namespace Skywalker.Application.Abstractions;

public interface IExecuteHandlerProvider<TOutputDto> /*: IScopedDependency*/ where TOutputDto : IEntityDto
{
    Task<TOutputDto?> HandleAsync(CancellationToken cancellationToken = default);
}

public interface IExecuteHandlerProvider<TInputDto, TOutputDto> /*: IScopedDependency*/ where TInputDto : IEntityDto where TOutputDto : IEntityDto
{
    Task<TOutputDto?> HandleAsync(TInputDto inputDto, CancellationToken cancellationToken = default);
}
