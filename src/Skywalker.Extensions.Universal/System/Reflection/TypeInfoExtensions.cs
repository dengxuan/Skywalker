namespace System.Reflection;

/// <summary>
/// https://github.com/castleproject/Core/blob/netcore/src/Castle.Core/Compatibility/IntrospectionExtensions.cs
/// </summary>
public static class TypeInfoExtensions
{
    public static Type[] GetGenericTypeArguments(this TypeInfo typeInfo)
    {
        return typeInfo.GenericTypeArguments;
    }
}
