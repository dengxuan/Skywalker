using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Skywalker.Extensions.VirtualFileSystem.Embedded;
using Skywalker.Extensions.VirtualFileSystem.Physical;

namespace Skywalker.Extensions.VirtualFileSystem;

public static class VirtualFileSetListExtensions
{
    public static void AddEmbedded<T>(this VirtualFileSetList list, string? baseNamespace = null, string? baseFolder = null)
    {
        list.NotNull(nameof(list));

        var assembly = typeof(T).Assembly;
        var fileProvider = CreateFileProvider(
            assembly,
            baseNamespace,
            baseFolder
        );

        list.Add(new EmbeddedVirtualFileSetInfo(fileProvider, assembly, baseFolder));
    }

    public static void AddPhysical(this VirtualFileSetList list, string root, ExclusionFilters exclusionFilters = ExclusionFilters.Sensitive)
    {
        list.NotNull(nameof(list));
        root.NotNullOrWhiteSpace(nameof(root));

        var fileProvider = new PhysicalFileProvider(root, exclusionFilters);
        list.Add(new PhysicalVirtualFileSetInfo(fileProvider, root));
    }

    private static IFileProvider CreateFileProvider(Assembly assembly, string? baseNamespace = null, string? baseFolder = null)
    {
        assembly.NotNull(nameof(assembly));

        var info = assembly.GetManifestResourceInfo("Microsoft.Extensions.FileProviders.Embedded.Manifest.xml");

        if (info == null)
        {
            return new AbpEmbeddedFileProvider(assembly, baseNamespace);
        }

        if (baseFolder == null)
        {
            return new ManifestEmbeddedFileProvider(assembly);
        }

        return new ManifestEmbeddedFileProvider(assembly, baseFolder);
    }

    public static void ReplaceEmbeddedByPhysical<T>(
        this VirtualFileSetList fileSets,
        string physicalPath)
    {
        fileSets.NotNull(nameof(fileSets));
        physicalPath.NotNullOrWhiteSpace(nameof(physicalPath));

        var assembly = typeof(T).Assembly;

        for (var i = 0; i < fileSets.Count; i++)
        {
            if (fileSets[i] is EmbeddedVirtualFileSetInfo embeddedVirtualFileSet &&
                embeddedVirtualFileSet.Assembly == assembly)
            {
                var thisPath = physicalPath;

                if (!embeddedVirtualFileSet.BaseFolder.IsNullOrEmpty())
                {
                    thisPath = Path.Combine(thisPath, embeddedVirtualFileSet.BaseFolder);
                }

                fileSets[i] = new PhysicalVirtualFileSetInfo(new PhysicalFileProvider(thisPath), thisPath);
            }
        }
    }
}
