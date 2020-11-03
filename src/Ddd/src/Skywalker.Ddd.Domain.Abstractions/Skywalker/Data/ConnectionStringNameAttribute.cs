﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Skywalker.Data
{
    public class ConnectionStringNameAttribute : Attribute
    {
        [NotNull]
        public string Name { get; }

        public ConnectionStringNameAttribute([NotNull] string name)
        {
            Check.NotNull(name, nameof(name));

            Name = name;
        }

        public static string GetConnStringName<T>()
        {
            return GetConnStringName(typeof(T));
        }

        public static string GetConnStringName(Type type)
        {
            var nameAttribute = type.GetTypeInfo().GetCustomAttribute<ConnectionStringNameAttribute>();

            if (nameAttribute == null)
            {
                return type.FullName;
            }

            return nameAttribute.Name;
        }
    }
}