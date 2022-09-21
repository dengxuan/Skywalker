using Skywalker.Ddd.Application.Abstractions;

namespace Skywalker.Ddd.ApplicationService.Examples;

public interface IExampleApplicationService : IApplicationService
{
    ValueTask GetValueAsync(int id, string name);
}
