namespace Skywalker.BlobStoring.Abstractions;

/// <summary>
/// 
/// </summary>
public class AbpBlobStoringOptions
{
    /// <summary>
    /// 
    /// </summary>
    public BlobContainerConfigurations Containers { get; }

    /// <summary>
    /// 
    /// </summary>
    public AbpBlobStoringOptions()
    {
        Containers = new BlobContainerConfigurations();
    }
}
