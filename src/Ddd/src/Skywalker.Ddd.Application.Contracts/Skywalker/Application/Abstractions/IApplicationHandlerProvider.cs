using Skywalker.Application.Dtos.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.Application.Abstractions
{
    public interface IApplicationHandlerProvider<TOutputDto> where TOutputDto : IEntityDto
    {
        Task<TOutputDto?> HandleAsync(CancellationToken cancellationToken = default);
    }

    public interface IApplicationHandlerProvider<TInputDto, TOutputDto> where TInputDto : IEntityDto where TOutputDto : IEntityDto
    {
        Task<TOutputDto?> HandleAsync(TInputDto inputDto, CancellationToken cancellationToken = default);
    }
}
