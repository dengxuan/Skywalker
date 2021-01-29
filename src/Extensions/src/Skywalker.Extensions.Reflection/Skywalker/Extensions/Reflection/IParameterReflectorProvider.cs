namespace Skywalker.Extensions.Reflection
{
    public interface IParameterReflectorProvider
    {
        ParameterReflector[] ParameterReflectors { get; }
    }
}