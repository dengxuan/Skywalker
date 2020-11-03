using System.Threading.Tasks;

namespace Skywalker.Application.Services.Abstractions
{
    public interface IDeleteAppService<in TKey> : IApplicationService
    {
        Task DeleteAsync(TKey id);
    }
}
