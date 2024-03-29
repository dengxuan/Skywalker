﻿using System.Collections.Concurrent;
using System.Reflection;

namespace Skywalker.Extensions.Linq.Parser;

internal static class EnumerationsFromMscorlib
{
    /// <summary>
    /// All Enum types from mscorlib/netstandard.
    /// </summary>
    public static readonly IDictionary<string, Type> PredefinedEnumerationTypes = new ConcurrentDictionary<string, Type>();

    static EnumerationsFromMscorlib()
    {
        var list = new List<Type>(AddEnumsFromAssembly(typeof(UriFormat).GetTypeInfo().Assembly.FullName!));

        list.AddRange(AddEnumsFromAssembly("System.Runtime"));
        list.AddRange(AddEnumsFromAssembly("System.Private.Corelib"));
        foreach (var group in list.GroupBy(t => t.Name))
        {
            Add(group);
        }
    }

    private static IEnumerable<Type> AddEnumsFromAssembly(string assemblyName)
    {
        try
        {
            return Assembly.Load(new AssemblyName(assemblyName)).GetTypes().Where(t => t.GetTypeInfo().IsEnum && t.GetTypeInfo().IsPublic);
        }
        catch
        {
            return Enumerable.Empty<Type>();
        }
    }

    private static void Add(IGrouping<string, Type> group)
    {
        if (group.Count() == 1)
        {
            var singleType = group.Single();
            PredefinedEnumerationTypes.Add(group.Key, singleType);
            PredefinedEnumerationTypes.Add(singleType.FullName!, singleType);
        }
        else
        {
            foreach (var fullType in group)
            {
                PredefinedEnumerationTypes.Add(fullType.FullName!, fullType);
            }
        }
    }
}
