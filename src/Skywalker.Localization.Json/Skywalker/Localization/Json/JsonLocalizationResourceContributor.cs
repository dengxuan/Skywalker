using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Skywalker.Extensions.VirtualFileSystem;

namespace Skywalker.Localization.Json;

/// <summary>
/// Contributes localized strings from JSON files.
/// </summary>
public class JsonLocalizationResourceContributor : ILocalizationResourceContributor
{
    private readonly string _virtualPath;
    private readonly string _fileExtension;

    // Culture -> (Key -> Value)
    private Dictionary<string, Dictionary<string, string>>? _dictionaries;

    /// <summary>
    /// Creates a new instance of <see cref="JsonLocalizationResourceContributor"/>.
    /// </summary>
    /// <param name="virtualPath">The virtual path to the JSON resource files.</param>
    /// <param name="fileExtension">The file extension (default: .json).</param>
    public JsonLocalizationResourceContributor(string virtualPath, string fileExtension = ".json")
    {
        _virtualPath = virtualPath ?? throw new ArgumentNullException(nameof(virtualPath));
        _fileExtension = fileExtension;
    }

    /// <inheritdoc/>
    public void Initialize(LocalizationResourceInitializationContext context)
    {
        _dictionaries = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        LoadResources(GetCandidateFileProviders(context.ServiceProvider));
    }

    /// <inheritdoc/>
    public LocalizedString? GetOrNull(string cultureName, string name)
    {
        if (_dictionaries == null)
        {
            return null;
        }

        if (_dictionaries.TryGetValue(cultureName, out var dictionary) &&
            dictionary.TryGetValue(name, out var value))
        {
            return new LocalizedString(name, value);
        }

        return null;
    }

    /// <inheritdoc/>
    public void Fill(string cultureName, Dictionary<string, LocalizedString> dictionary)
    {
        if (_dictionaries == null)
        {
            return;
        }

        if (_dictionaries.TryGetValue(cultureName, out var source))
        {
            foreach (var (key, value) in source)
            {
                dictionary[key] = new LocalizedString(key, value);
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetSupportedCultures()
    {
        return _dictionaries?.Keys ?? Enumerable.Empty<string>();
    }

    /// <summary>
    /// Candidate providers in priority order: explicit <see cref="IFileProvider"/> → <see cref="IVirtualFileProvider"/>
    /// (registered by <c>AddVirtualFileSystem</c> under its own service type, #311) → host content root
    /// (ASP.NET Core registers no bare IFileProvider) → physical base directory (console apps, tests).
    /// </summary>
    private static IEnumerable<IFileProvider> GetCandidateFileProviders(IServiceProvider serviceProvider)
    {
        if (serviceProvider.GetService(typeof(IFileProvider)) is IFileProvider fileProvider)
        {
            yield return fileProvider;
        }

        if (serviceProvider.GetService(typeof(IVirtualFileProvider)) is IVirtualFileProvider virtualFileProvider)
        {
            yield return virtualFileProvider;
        }

        if (serviceProvider.GetService(typeof(IHostEnvironment)) is IHostEnvironment { ContentRootFileProvider: { } contentRoot })
        {
            yield return contentRoot;
        }

        yield return new PhysicalFileProvider(AppContext.BaseDirectory);
    }

    private void LoadResources(IEnumerable<IFileProvider> fileProviders)
    {
        if (_dictionaries == null)
        {
            return;
        }

        // Use the first provider that actually has the resource directory, so e.g. a VFS registered only for
        // templates does not shadow JSON files that live under the content root.
        var directoryContents = fileProviders
            .Select(GetDirectoryContents)
            .FirstOrDefault(contents => contents.Exists);
        if (directoryContents == null)
        {
            return;
        }

        foreach (var file in directoryContents)
        {
            if (file.IsDirectory || !file.Name.EndsWith(_fileExtension, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var cultureName = GetCultureFromFileName(file.Name);
            var content = ReadFileContent(file);

            if (!string.IsNullOrEmpty(content))
            {
                var dictionary = JsonLocalizationDictionaryBuilder.Build(content);
                _dictionaries[cultureName] = dictionary;
            }
        }
    }

    private IDirectoryContents GetDirectoryContents(IFileProvider fileProvider)
    {
        // PhysicalFileProvider wants relative paths; the virtual-path convention here is "/Localization/Res".
        var directoryContents = fileProvider.GetDirectoryContents(_virtualPath);
        if (!directoryContents.Exists && _virtualPath.StartsWith('/'))
        {
            directoryContents = fileProvider.GetDirectoryContents(_virtualPath.TrimStart('/'));
        }

        return directoryContents;
    }

    private string GetCultureFromFileName(string fileName)
    {
        // Remove extension: "en.json" -> "en", "zh-CN.json" -> "zh-CN"
        var name = fileName[..^_fileExtension.Length];
        return name;
    }

    private static string ReadFileContent(IFileInfo file)
    {
        using var stream = file.CreateReadStream();
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}

