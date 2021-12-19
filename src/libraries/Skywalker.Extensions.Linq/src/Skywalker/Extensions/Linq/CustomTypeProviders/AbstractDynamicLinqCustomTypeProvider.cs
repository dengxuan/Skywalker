using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Skywalker.Extensions.Linq.CustomTypeProviders
{
    /// <summary>
    /// The abstract DynamicLinqCustomTypeProvider which is used by the DefaultDynamicLinqCustomTypeProvider and can be used by a custom TypeProvider like in .NET Core.
    /// </summary>
    public abstract class AbstractDynamicLinqCustomTypeProvider
    {
        /// <summary>
        /// Finds the unique types marked with DynamicLinqTypeAttribute.
        /// </summary>
        /// <param name="assemblies">The assemblies to process.</param>
        /// <returns><see cref="IEnumerable{Type}" /></returns>
        protected IEnumerable<Type?> FindTypesMarkedWithDynamicLinqTypeAttribute(IEnumerable<Assembly> assemblies)
        {
            Check.NotNull(assemblies, nameof(assemblies));
            assemblies = assemblies.Where(a => !a.IsDynamic);
            return GetAssemblyTypesWithDynamicLinqTypeAttribute(assemblies).Distinct().ToArray();
        }

        /// <summary>
        /// Resolve any type which is registered in the current application domain.
        /// </summary>
        /// <param name="assemblies">The assemblies to inspect.</param>
        /// <param name="typeName">The typename to resolve.</param>
        /// <returns>A resolved <see cref="Type"/> or null when not found.</returns>
        protected static Type? ResolveType(IEnumerable<Assembly> assemblies, string typeName)
        {
            Check.NotNull(assemblies, nameof(assemblies));
            Check.NotNullOrEmpty(typeName, nameof(typeName));

            foreach (Assembly? assembly in assemblies)
            {
                Type? resolvedType = assembly.GetType(typeName, false, true);
                if (resolvedType != null)
                {
                    return resolvedType;
                }
            }

            return null;
        }

        /// <summary>
        /// Resolve a type by the simple name which is registered in the current application domain.
        /// </summary>
        /// <param name="assemblies">The assemblies to inspect.</param>
        /// <param name="simpleTypeName">The simple typename to resolve.</param>
        /// <returns>A resolved <see cref="Type"/> or null when not found.</returns>
        protected static Type? ResolveTypeBySimpleName(IEnumerable<Assembly> assemblies, string simpleTypeName)
        {
            Check.NotNull(assemblies, nameof(assemblies));
            Check.NotNullOrEmpty(simpleTypeName, nameof(simpleTypeName));

            foreach (Assembly? assembly in assemblies)
            {
                IEnumerable<string?>? fullnames = assembly.GetTypes().Select(t => t.FullName).Distinct();
                string? firstMatchingFullname = fullnames.FirstOrDefault(fn => fn?.EndsWith($".{simpleTypeName}") == true);

                if (firstMatchingFullname != null)
                {
                    Type? resolvedType = assembly.GetType(firstMatchingFullname, false, true);
                    if (resolvedType != null)
                    {
                        return resolvedType;
                    }
                }
            }

            return null;
        }

#if (WINDOWS_APP || UAP10_0 || NETSTANDARD || NET5_0_OR_GREATER || NETCOREAPP)
        /// <summary>
        /// Gets the assembly types annotated with <see cref="DynamicLinqTypeAttribute"/> in an Exception friendly way.
        /// </summary>
        /// <param name="assemblies">The assemblies to process.</param>
        /// <returns><see cref="IEnumerable{Type}" /></returns>
        protected IEnumerable<Type?> GetAssemblyTypesWithDynamicLinqTypeAttribute(IEnumerable<Assembly> assemblies)
        {
            Check.NotNull(assemblies, nameof(assemblies));

            foreach (Assembly? assembly in assemblies)
            {
                Type?[]? definedTypes = null;

                try
                {
                    definedTypes = assembly.ExportedTypes.Where(t => t.GetTypeInfo().IsDefined(typeof(DynamicLinqTypeAttribute), false)).ToArray();
                }
                catch (ReflectionTypeLoadException reflectionTypeLoadException)
                {
                    definedTypes = reflectionTypeLoadException.Types;
                }
                catch
                {
                    // Ignore all other exceptions
                }

                if (definedTypes != null && definedTypes.Length > 0)
                {
                    foreach (Type? definedType in definedTypes)
                    {
                        yield return definedType;
                    }
                }
            }
        }
#else                
        /// <summary>
        /// Gets the assembly types annotated with <see cref="DynamicLinqTypeAttribute"/> in an Exception friendly way.
        /// </summary>
        /// <param name="assemblies">The assemblies to process.</param>
        /// <returns><see cref="IEnumerable{Type}" /></returns>
        protected IEnumerable<Type> GetAssemblyTypesWithDynamicLinqTypeAttribute(IEnumerable<Assembly> assemblies)
        {
            Check.NotNull(assemblies, nameof(assemblies));

            foreach (var assembly in assemblies.Where(a => !a.GlobalAssemblyCache)) // Skip System DLL's
            {
                Type[]? definedTypes = null;

                try
                {
                    definedTypes = assembly.GetExportedTypes()
                        .Where(t => t.IsDefined(typeof(DynamicLinqTypeAttribute), false)).ToArray();
                }
                catch (ReflectionTypeLoadException reflectionTypeLoadException)
                {
                    definedTypes = reflectionTypeLoadException.Types;
                }
                catch
                {
                    // Ignore all other exceptions
                }

                if (definedTypes != null && definedTypes.Length > 0)
                {
                    foreach (var definedType in definedTypes)
                    {
                        yield return definedType;
                    }
                }
            }
        }
#endif
    }
}
