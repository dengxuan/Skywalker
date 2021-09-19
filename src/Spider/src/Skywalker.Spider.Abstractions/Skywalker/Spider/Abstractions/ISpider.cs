using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Spider.Abstractions;

public interface ISpider<out TRequestSupplier> where TRequestSupplier : IRequestSupplier
{
    Task InitializeAsync(CancellationToken cancellationToken);

    Task ExecuteAsync(CancellationToken cancellationToken);

    Task UnInitializeAsync(CancellationToken cancellationToken);
}
