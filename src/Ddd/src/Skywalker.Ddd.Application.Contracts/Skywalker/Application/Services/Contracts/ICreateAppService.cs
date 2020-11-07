using System.Threading.Tasks;

namespace Skywalker.Application.Services.Contracts
{
    public interface ICreateAppService<TEntityDto>
        : ICreateAppService<TEntityDto, TEntityDto>
    {

    }

    public interface ICreateAppService<TGetOutputDto, in TCreateInput>
        : IApplicationService
    {
        Task<TGetOutputDto> CreateAsync(TCreateInput input);
    }
}
