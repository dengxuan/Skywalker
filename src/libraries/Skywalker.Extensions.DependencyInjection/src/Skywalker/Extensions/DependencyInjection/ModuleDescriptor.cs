// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Reflection;

namespace Skywalker.Extensions.DependencyInjection;

internal class ModuleDescriptor: IModuleDescriptor
{
    private readonly List<IModuleDescriptor> _dependencies;

    public Type Type { get; }

    public Assembly Assembly { get; }

    public Modular Instance { get; }

    public IReadOnlyList<IModuleDescriptor> Dependencies => _dependencies.ToImmutableList();

    public ModuleDescriptor(Type type, Modular instance)
    {
        Check.NotNull(type, nameof(type));
        Check.NotNull(instance, nameof(instance));

        if (!type.GetTypeInfo().IsAssignableFrom(instance.GetType()))
        {
            throw new ArgumentException($"Given module instance ({instance.GetType().AssemblyQualifiedName}) is not an instance of given module type: {type.AssemblyQualifiedName}");
        }

        Type = type;
        Assembly = type.Assembly;
        Instance = instance;

        _dependencies = new List<IModuleDescriptor>();
    }

    public void AddDependency(ModuleDescriptor descriptor)
    {
        _dependencies.AddIfNotContains(descriptor);
    }

    public override string ToString()
    {
        return $"[ModuleDescriptor {Type.FullName}]";
    }
}
