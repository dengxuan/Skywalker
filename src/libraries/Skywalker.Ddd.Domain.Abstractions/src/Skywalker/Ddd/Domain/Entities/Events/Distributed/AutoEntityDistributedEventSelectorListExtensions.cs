using Skywalker.ExceptionHandler;
using Skywalker.Extensions.Collections;

namespace Skywalker.Ddd.Domain.Entities.Events.Distributed;

public static class AutoEntityDistributedEventSelectorListExtensions
{
    public const string AllEntitiesSelectorName = "All";

    public static void AddNamespace(this IAutoEntityDistributedEventSelectorList selectors, string namespaceName)
    {
        selectors.NotNull(nameof(selectors));

        var selectorName = "Namespace:" + namespaceName;
        if (selectors.Any(s => s.Name == selectorName))
        {
            return;
        }

        selectors.Add(
            new NamedTypeSelector(
                selectorName,
                t => t.FullName?.StartsWith(namespaceName) ?? false
            )
        );
    }

    /// <summary>
    /// Adds a specific entity type and the types derived from that entity type.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity</typeparam>
    public static void Add<TEntity>(this IAutoEntityDistributedEventSelectorList selectors)
        where TEntity : IEntity
    {
        selectors.NotNull(nameof(selectors));

        var selectorName = "Entity:" + typeof(TEntity).FullName;
        if (selectors.Any(s => s.Name == selectorName))
        {
            return;
        }

        selectors.Add(
            new NamedTypeSelector(
                selectorName,
                t => typeof(TEntity).IsAssignableFrom(t)
            )
        );
    }

    /// <summary>
    /// Adds all entity types.
    /// </summary>
    public static void AddAll(this IAutoEntityDistributedEventSelectorList selectors)
    {
        selectors.NotNull(nameof(selectors));

        if (selectors.Any(s => s.Name == AllEntitiesSelectorName))
        {
            return;
        }

        selectors.Add(
            new NamedTypeSelector(
                AllEntitiesSelectorName,
                t => typeof(IEntity).IsAssignableFrom(t)
            )
        );
    }

    public static void Add(
        this IAutoEntityDistributedEventSelectorList selectors,
        string selectorName,
        Func<Type, bool> predicate)
    {
        selectors.NotNull(nameof(selectors));

        if (selectors.Any(s => s.Name == selectorName))
        {
            throw new SkywalkerException($"There is already a selector added before with the same name: {selectorName}");
        }

        selectors.Add(
            new NamedTypeSelector(
                selectorName,
                predicate
            )
        );
    }

    public static void Add(
        this IAutoEntityDistributedEventSelectorList selectors,
        Func<Type, bool> predicate)
    {
        selectors.Add(Guid.NewGuid().ToString("N"), predicate);
    }

    public static bool RemoveByName(
        this IAutoEntityDistributedEventSelectorList selectors,
        string name)
    {
        selectors.NotNull(nameof(selectors));
        name.NotNull(nameof(name));

        return selectors.RemoveAll(s => s.Name == name).Count > 0;
    }


    public static bool IsMatch(this IAutoEntityDistributedEventSelectorList selectors, Type entityType)
    {
        selectors.NotNull(nameof(selectors));
        return selectors.Any(s => s.Predicate(entityType));
    }
}
