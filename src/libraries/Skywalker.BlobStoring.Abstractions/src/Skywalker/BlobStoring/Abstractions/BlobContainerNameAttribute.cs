﻿using System.Reflection;


namespace Skywalker.BlobStoring.Abstractions;

[AttributeUsage(AttributeTargets.Class)]
public sealed class BlobContainerNameAttribute : Attribute
{
    
    public string Name { get; }

    public BlobContainerNameAttribute(string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));

        Name = name;
    }

    public string GetName(Type type)
    {
        return Name;
    }

    public static string GetContainerName<T>()
    {
        return GetContainerName(typeof(T));
    }

    public static string GetContainerName(Type type)
    {
        var nameAttribute = type.GetCustomAttribute<BlobContainerNameAttribute>();

        if (nameAttribute == null)
        {
            return type.FullName!;
        }

        return nameAttribute.GetName(type);
    }
}
