using Microsoft.Extensions.DependencyInjection;
using Skywalker.Application.Dtos.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Application.Abstractions;

public interface IExecuteNonQueryHandlerProvider<TInputDto> : IScopedDependency where TInputDto : IEntityDto
{
    Task HandleAsync(TInputDto inputDto, CancellationToken cancellationToken = default);
}