using Skywalker.Reflection;
using System;
using System.Linq;
using System.Reflection;

namespace Skywalker.Aspects.Interceptors
{
    public class ProxyHelper
    {
        private const string ProxyNamespace = "Skywalker.Proxies";

        /// <summary>
        /// Returns dynamic proxy target object if this is a proxied object, otherwise returns the given object. 
        /// It supports Castle Dynamic Proxies.
        /// </summary>
        public static object UnProxy(object obj)
        {
            if (obj.GetType().Namespace != ProxyNamespace)
            {
                return obj;
            }

            var targetField = obj.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(f => f.Name == "__target");

            if (targetField == null)
            {
                return obj;
            }

            return targetField.GetValue(obj);
        }

        public static Type GetUnProxiedType(object obj)
        {
            return UnProxy(obj).GetType();
        }

        public static bool ShouldProxy(Type serviceType)
        {
            if (typeof(IAspects).IsAssignableFrom(serviceType))
            {
                var attribute = CustomAttributeAccessor.GetCustomAttribute<AspectsAttribute>(serviceType, true);
                return attribute?.Disable != true;
            }
            return false;
        }
    }
}
