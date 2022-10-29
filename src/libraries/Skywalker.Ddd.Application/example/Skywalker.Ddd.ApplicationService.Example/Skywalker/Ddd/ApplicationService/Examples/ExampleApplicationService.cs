using Skywalker.Ddd.ApplicationService.Example.Abstractions.Skywalker.Ddd.ApplicationService.Examples;

namespace Skywalker.Ddd.ApplicationService.Examples;

internal class ExampleApplicationService : Application.ApplicationService, IExampleApplicationService, IExample1ApplicationService
{
    public ValueTask<GetExampleResponseDto> GetValue1Async(GetExampleRequestDto request)
    {
        return ValueTask.FromResult(new GetExampleResponseDto($"{request.Id}+{request.Name}"));
    }

    public ValueTask<GetExampleResponseDto> GetValueAsync(GetExampleRequestDto request)
    {
        return ValueTask.FromResult(new GetExampleResponseDto($"{request.Id}-{request.Name}"));
    }
}
