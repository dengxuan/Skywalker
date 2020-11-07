using System.Threading.Tasks;

namespace Skywalker.Application.Services.Contracts
{
    public interface IDeleteAppService<in TKey> : IApplicationService
    {
        Task DeleteAsync(TKey id);
    }
}
