using System.Reflection;

namespace Skywalker.Extensions.Reflection
{
    public partial class CustomAttributeReflector
    {
        internal static CustomAttributeReflector Create(CustomAttributeData customAttributeData)
        {
            return ReflectorCacheUtils<CustomAttributeData, CustomAttributeReflector>.GetOrAdd(customAttributeData, data => new CustomAttributeReflector(data));
        }
    }
}
