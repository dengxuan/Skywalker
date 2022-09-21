namespace Skywalker.Ddd.ApplicationService.Examples;

internal sealed class ExampleApplicationService : IExampleApplicationService
{
    public ValueTask GetValueAsync()
    {
        Console.WriteLine(1);
        return ValueTask.CompletedTask;
    }
}
