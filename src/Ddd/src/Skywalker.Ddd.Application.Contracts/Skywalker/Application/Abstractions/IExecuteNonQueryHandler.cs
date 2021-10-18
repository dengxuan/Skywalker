using Microsoft.Extensions.DependencyInjection;
using Skywalker.Application.Dtos.Contracts;
using Skywalker.Ddd.UnitOfWork.Abstractions;

namespace Skywalker.Application.Abstractions;

public interface IExecuteNonQueryHandler<in TInputDto> : IUnitOfWorkEnabled, IScopedDependency where TInputDto : IEntityDto
{
    Task HandleAsync(TInputDto inputDto, CancellationToken cancellationToken);
}