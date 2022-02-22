using JetBrains.Annotations;

namespace Skywalker.Authorization.Permissions;

public class PermissionGrantInfo
{
    public string Name { get; }

    public bool IsGranted { get; }

    public string ProviderName { get; }

    public string ProviderKey { get; }

    public PermissionGrantInfo([NotNull] string name, bool isGranted, [CanBeNull] string providerName = null, [CanBeNull] string providerKey = null)
    {
        name.NotNull(nameof(name));

        Name = name;
        IsGranted = isGranted;
        ProviderName = providerName;
        ProviderKey = providerKey;
    }
}
