﻿using System.Reflection;
using System.Reflection.Emit;

namespace Skywalker.Extensions.Linq;

internal static class AssemblyBuilderFactory
{
    /// <summary>
    /// Defines a dynamic assembly that has the specified name and access rights.
    /// </summary>
    /// <param name="name">The name of the assembly.</param>
    /// <param name="access">The access rights of the assembly.</param>
    /// <returns>An object that represents the new assembly.</returns>
    public static AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access)
    {
        return AssemblyBuilder.DefineDynamicAssembly(name, access);
    }
}
