namespace Skywalker.Ddd.Infrastructure.Abstractions
{
    public interface IDataCollection<TCollection>
    {
        TCollection Collection { get; set; }
    }
}
