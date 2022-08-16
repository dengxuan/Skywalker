// Licensed to the zshiot.com under one or more agreements.
// zshiot.com licenses this file to you under the license.

using System;
using System.Collections.Generic;
using System.Text;

namespace Skywalker.BlobStoring.FileSystem;

/// <summary>
/// A helper class for Directory operations.
/// </summary>
public static class DirectoryHelper
{
    public static void CreateIfNotExists(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public static void DeleteIfExists(string directory)
    {
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory);
        }
    }

    public static void DeleteIfExists(string directory, bool recursive)
    {
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, recursive);
        }
    }

    public static void CreateIfNotExists(DirectoryInfo directory)
    {
        if (!directory.Exists)
        {
            directory.Create();
        }
    }

    public static bool IsSubDirectoryOf( string parentDirectoryPath,  string childDirectoryPath)
    {
        Check.NotNull(parentDirectoryPath, nameof(parentDirectoryPath));
        Check.NotNull(childDirectoryPath, nameof(childDirectoryPath));

        return IsSubDirectoryOf(
            new DirectoryInfo(parentDirectoryPath),
            new DirectoryInfo(childDirectoryPath)
        );
    }

    public static bool IsSubDirectoryOf(DirectoryInfo parentDirectory,
        DirectoryInfo childDirectory)
    {
        Check.NotNull(parentDirectory, nameof(parentDirectory));
        Check.NotNull(childDirectory, nameof(childDirectory));

        if (parentDirectory.FullName == childDirectory.FullName)
        {
            return true;
        }

        var parentOfChild = childDirectory.Parent;
        if (parentOfChild == null)
        {
            return false;
        }

        return IsSubDirectoryOf(parentDirectory, parentOfChild);
    }

    public static IDisposable ChangeCurrentDirectory(string targetDirectory)
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        if (currentDirectory.Equals(targetDirectory, StringComparison.OrdinalIgnoreCase))
        {
            return NullDisposable.Instance;
        }

        Directory.SetCurrentDirectory(targetDirectory);

        return new DisposeAction(() => { Directory.SetCurrentDirectory(currentDirectory); });
    }
}
