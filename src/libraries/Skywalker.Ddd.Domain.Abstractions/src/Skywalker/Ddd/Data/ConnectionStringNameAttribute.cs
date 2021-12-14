using System.Reflection;

namespace Skywalker.Data
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConnectionStringNameAttribute : Attribute
    {
        
        public string Name { get; }

        public ConnectionStringNameAttribute(string name)
        {
            Check.NotNull(name, nameof(name));

            Name = name;
        }

        public static string GetConnectionStringName<T>()
        {
            return GetConnectionStringName(typeof(T));
        }

        public static string GetConnectionStringName(Type type)
        {
            var nameAttribute = type.GetCustomAttribute<ConnectionStringNameAttribute>();

            if (nameAttribute == null)
            {
                return type.FullName!;
            }

            return nameAttribute.Name;
        }
    }
}