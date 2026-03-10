using Microsoft.Extensions.FileProviders;

namespace Skywalker.Localization.Json;

/// <summary>
/// Contributes localized strings from JSON files.
/// </summary>
public class JsonLocalizationResourceContributor : ILocalizationResourceContributor
{
    private readonly string _virtualPath;
    private readonly string _fileExtension;
    private IFileProvider? _fileProvider;

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
        _fileProvider = context.ServiceProvider.GetService(typeof(IFileProvider)) as IFileProvider;
        _dictionaries = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        LoadResources();
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

    private void LoadResources()
    {
        if (_fileProvider == null || _dictionaries == null)
        {
            return;
        }

        var directoryContents = _fileProvider.GetDirectoryContents(_virtualPath);
        if (!directoryContents.Exists)
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

