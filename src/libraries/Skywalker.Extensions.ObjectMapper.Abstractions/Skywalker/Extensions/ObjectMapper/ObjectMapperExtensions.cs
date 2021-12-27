using System.Reflection;

namespace Skywalker.Extensions.ObjectMapper;

public static class ObjectMapperExtensions
{
    private static readonly MethodInfo? s_mapToNewObjectMethod;
    private static readonly MethodInfo? s_mapToExistingObjectMethod;

    static ObjectMapperExtensions()
    {
        var methods = typeof(IObjectMapper).GetMethods();
        foreach (var method in methods)
        {
            if (method.Name == nameof(IObjectMapper.Map) && method.IsGenericMethodDefinition)
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 1)
                {
                    s_mapToNewObjectMethod = method;
                }
                else if (parameters.Length == 2)
                {
                    s_mapToExistingObjectMethod = method;
                }
            }
        }
    }

    public static object? Map(this IObjectMapper objectMapper, Type sourceType, Type destinationType, object source)
    {
        return s_mapToNewObjectMethod?
            .MakeGenericMethod(sourceType, destinationType)
            .Invoke(objectMapper, new[] { source });
    }

    public static object? Map(this IObjectMapper objectMapper, Type sourceType, Type destinationType, object source, object destination)
    {
        return s_mapToExistingObjectMethod?
            .MakeGenericMethod(sourceType, destinationType)
            .Invoke(objectMapper, new[] { source, destination });
    }
}
