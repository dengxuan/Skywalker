namespace Skywalker.Extensions.DependencyInjection.Interceptors.Abstractions
{
    internal interface IOrderedSequenceItem
    {
        int Order { get; set; }
    }
}
