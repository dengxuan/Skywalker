namespace Skywalker.Serialization.Abstractions
{
    public interface ISerializer
    {
        string Serialize(object @object, bool camelCase = true, bool indented = false);

        T? Deserialize<T>(byte[] bytes, bool camelCase = true);

        object? Deserialize(Type type, byte[] bytes, bool camelCase = true);
    }
}
