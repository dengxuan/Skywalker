// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Skywalker.Extensions.DependencyInjection.Generators;

public partial class DependencyInjectionGenerator
{
    internal readonly record struct Method
    {
        public Method(string name, string returnType)
        {
            Name = name;
            ReturnType = returnType;
        }

        public List<KeyValuePair<string, string>> Arguments { get; } = new();

        public string ReturnType { get; }

        public string Name { get; }
    }

    internal readonly record struct Dependency
    {

        public string Name { get; }

        public ISet<string> Interfaces { get; } = new HashSet<string>();

        public Dependency(string name)
        {
            Name = name;
        }
    }

    internal readonly record struct Intecepter
    {
        public string Name { get; }

        public ISet<string> Interfaces { get; } = new HashSet<string>();

        public List<Method> Methods { get; } = new();

        public Intecepter(string name)
        {
            Name = name;
        }
    }

    internal readonly record struct Metadata
    {
        public Metadata() { }

        public ISet<string> Namespaces { get; } = new HashSet<string>();

        public ISet<Dependency> ScopedDepedency { get; } = new HashSet<Dependency>();

        public ISet<Dependency> SingletonDepedency { get; } = new HashSet<Dependency>();

        public ISet<Dependency> TransientDepedency { get; } = new HashSet<Dependency>();

        public ISet<Intecepter> Intecepters { get; } = new HashSet<Intecepter>();
    }
}
