namespace System.Reflection.Emit
{
    /// <summary>
    /// https://github.com/castleproject/Core/blob/netcore/src/Castle.Core/Compatibility/IntrospectionExtensions.cs
    /// </summary>
	public static class CustomIntrospectionExtensions
    {
        public static Type[] GetGenericTypeArguments(this TypeInfo typeInfo)
        {
            return typeInfo.GenericTypeArguments;
        }
    }
}
