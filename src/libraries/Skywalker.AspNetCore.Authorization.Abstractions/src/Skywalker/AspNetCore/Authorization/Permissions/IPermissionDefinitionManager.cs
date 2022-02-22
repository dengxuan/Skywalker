namespace Skywalker.Authorization.Permissions;

public interface IPermissionDefinitionManager
{
    PermissionDefinition Get(string name);

    PermissionDefinition? GetOrNull(string name);

    IReadOnlyList<PermissionDefinition> GetPermissions();

    IReadOnlyList<PermissionGroupDefinition> GetGroups();
}
