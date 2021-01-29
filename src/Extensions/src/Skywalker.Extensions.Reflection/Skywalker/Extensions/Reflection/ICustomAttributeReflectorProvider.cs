namespace Skywalker.Extensions.Reflection
{
    public interface ICustomAttributeReflectorProvider
    {
        CustomAttributeReflector[] CustomAttributeReflectors { get; }
    }
}