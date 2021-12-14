using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace System
{
    /// <summary>
    /// Extension methods for all objects.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Used to simplify and beautify casting an object to a type. 
        /// </summary>
        /// <typeparam name="T">Type to be casted</typeparam>
        /// <param name="obj">Object to cast</param>
        /// <returns>Casted object</returns>
        public static T As<T>(this object obj)
            where T : class
        {
            return (T)obj;
        }

        /// <summary>
        /// Converts given object to a value type using <see cref="Convert.ChangeType(object,TypeCode)"/> method.
        /// </summary>
        /// <param name="object">Object to be converted</param>
        /// <typeparam name="T">Type of the target object</typeparam>
        /// <returns>Converted object</returns>
        public static T To<T>(this object @object) where T : struct
        {
            if (typeof(T) == typeof(Guid))
            {
                return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(@object?.ToString()!)!;
            }

            return (T)Convert.ChangeType(@object, typeof(T), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Check if an item is in a list.
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <param name="list">List of items</param>
        /// <typeparam name="T">Type of the items</typeparam>
        public static bool IsIn<T>(this T item, params T[] list)
        {
            return list.Contains(item);
        }

        /// <summary>
        /// 获取枚举的描述信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static string GetEnumDescription<T>(this T @enum) where T : Enum
        {
            var type = @enum.GetType();
            string? name = Enum.GetName(type, @enum);
            if (name == null)
            {
                return string.Empty;
            }
            FieldInfo? field = type.GetField(name);
            if (field == null)
            {
                return string.Empty;
            }
            DescriptionAttribute? descriptionAttribute = field.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttribute == null)
            {
                return string.Empty;
            }

            return descriptionAttribute.Description;
        }

        /// <summary>
        /// 获取枚举的名字
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static string GetEnumName<T>(this T @enum) where T : Enum
        {
            var type = @enum.GetType();
            string? name = Enum.GetName(type, @enum);
            return name ?? string.Empty;
        }
    }
}
