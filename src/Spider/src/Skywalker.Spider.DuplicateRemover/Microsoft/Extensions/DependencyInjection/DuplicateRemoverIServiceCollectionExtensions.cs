using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Spider.DuplicateRemover;
using Skywalker.Spider.DuplicateRemover.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class DuplicateRemoverIServiceCollectionExtensions
{
    public static IServiceCollection AddBloomFilterDuplicateRemover(this IServiceCollection services)
    {
        services.AddSingleton<IDuplicateRemover, BloomFilterDuplicateRemover>();
        return services;
    }

    public static IServiceCollection AddHashSetDuplicateRemover(this IServiceCollection services)
    {
        services.AddSingleton<IDuplicateRemover, HashSetDuplicateRemover>();
        return services;
    }

    public static IServiceCollection AddDuplicateRemover(this IServiceCollection services)
    {
        services.TryAddSingleton<IDuplicateRemover, NoneDuplicateRemover>();
        return services;
    }
}
