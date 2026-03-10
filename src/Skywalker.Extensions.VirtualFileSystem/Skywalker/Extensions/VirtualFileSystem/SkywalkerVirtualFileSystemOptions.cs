using System.Text.Json.Serialization;

namespace Skywalker.Extensions.VirtualFileSystem;

/// <summary>
/// Options for virtual file system.
/// </summary>
public class SkywalkerVirtualFileSystemOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "Skywalker:VirtualFileSystem";

    /// <summary>
    /// Gets the file sets.
    /// This property is configured via code, not from configuration files.
    /// </summary>
    [JsonIgnore]
    public VirtualFileSetList FileSets { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SkywalkerVirtualFileSystemOptions"/> class.
    /// </summary>
    public SkywalkerVirtualFileSystemOptions()
    {
        FileSets = new VirtualFileSetList();
    }
}
