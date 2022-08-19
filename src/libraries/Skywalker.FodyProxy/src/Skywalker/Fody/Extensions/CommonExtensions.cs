namespace Skywalker.Fody.Extensions;

public static class CommonExtensions
{
    public static void MultiInvoke<T>(this Action<T> action, IList<T> items)
    {
        foreach (var item in items)
        {
            action(item);
        }
    }
}
