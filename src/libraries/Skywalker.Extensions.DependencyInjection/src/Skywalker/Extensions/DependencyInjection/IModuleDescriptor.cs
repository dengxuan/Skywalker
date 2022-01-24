// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Reflection;

namespace Skywalker.Extensions.DependencyInjection;

public interface IModuleDescriptor
{
    Type Type { get; }

    Assembly Assembly { get; }

    Modular Instance { get; }

    IReadOnlyList<IModuleDescriptor> Dependencies { get; }
}
