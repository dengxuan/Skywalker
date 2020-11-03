using Skywalker;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace System
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Find the best available name to describe a type.
        /// </summary>
        /// <remarks>
        /// Usually the best name will be <see cref="Type.FullName"/>, but
        /// sometimes that's null (see http://msdn.microsoft.com/en-us/library/system.type.fullname%28v=vs.110%29.aspx)
        /// in which case the method falls back to <see cref="MemberInfo.Name"/>.
        /// </remarks>
        /// <param name="type">the type to name</param>
        /// <returns>the best name</returns>
        public static string GetBestName(this Type type)
        {
            return type.AssemblyQualifiedName ?? type.FullName ?? type.Name;
        }

        public static Assembly GetAssembly(this Type type)
        {
            return type.GetTypeInfo().Assembly;
        }

        public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo memberInfo, bool inherit = true)
        {
            if (inherit)
            {
                return memberInfo.GetCustomAttributes(true).OfType<Attribute>().ToArray();
            }
            return memberInfo.GetCustomAttributes(false).OfType<Attribute>().ToArray();
        }
        public static IEnumerable<Attribute> GetCustomAttributes(this Type type, bool inherit = true)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            if (inherit)
            {
                return typeInfo.GetCustomAttributes(true).OfType<Attribute>().ToArray();
            }
            return typeInfo.GetCustomAttributes(false).OfType<Attribute>().ToArray();
        }

        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this MemberInfo memberInfo, bool inherit = true)
        {
            return GetCustomAttributes(memberInfo, inherit).OfType<TAttribute>();
        }

        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this Type type, bool inherit = true)
        {
            return GetCustomAttributes(type, inherit).OfType<TAttribute>();
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this MemberInfo memberInfo, bool inherit = true)
        {
            return GetCustomAttributes(memberInfo, inherit).OfType<TAttribute>().FirstOrDefault();
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this Type type, bool inherit = true)
        {
            return GetCustomAttributes(type, inherit).OfType<TAttribute>().FirstOrDefault();
        }

        public static string GetFullNameWithAssemblyName(this Type type)
        {
            return type.FullName + ", " + type.Assembly.GetName().Name;
        }

        /// <summary>
        /// Determines whether an instance of this type can be assigned to
        /// an instance of the <typeparamref name="TTarget"></typeparamref>.
        ///
        /// Internally uses <see cref="Type.IsAssignableFrom"/>.
        /// </summary>
        /// <typeparam name="TTarget">Target type</typeparam> (as reverse).
        public static bool IsAssignableTo<TTarget>([NotNull] this Type type)
        {
            Check.NotNull(type, nameof(type));

            return type.IsAssignableTo(typeof(TTarget));
        }

        /// <summary>
        /// Determines whether an instance of this type can be assigned to
        /// an instance of the <paramref name="targetType"></paramref>.
        ///
        /// Internally uses <see cref="Type.IsAssignableFrom"/> (as reverse).
        /// </summary>
        /// <param name="type">this type</param>
        /// <param name="targetType">Target type</param>
        public static bool IsAssignableTo([NotNull] this Type type, [NotNull] Type targetType)
        {
            Check.NotNull(type, nameof(type));
            Check.NotNull(targetType, nameof(targetType));

            return targetType.IsAssignableFrom(type);
        }

        /// <summary>
        /// Gets all base classes of this type.
        /// </summary>
        /// <param name="type">The type to get its base classes.</param>
        /// <param name="includeObject">True, to include the standard <see cref="object"/> type in the returned array.</param>
        public static Type[] GetBaseClasses([NotNull] this Type type, bool includeObject = true)
        {
            Check.NotNull(type, nameof(type));

            var types = new List<Type>();
            AddTypeAndBaseTypesRecursively(types, type.BaseType, includeObject);
            return types.ToArray();
        }

        private static void AddTypeAndBaseTypesRecursively([NotNull] List<Type> types, [MaybeNull] Type type, bool includeObject)
        {
            Check.NotNull(types, nameof(types));

            if (type == null)
            {
                return;
            }

            if (!includeObject && type == typeof(object))
            {
                return;
            }

            AddTypeAndBaseTypesRecursively(types, type.BaseType, includeObject);
            types.Add(type);
        }
    }
}
