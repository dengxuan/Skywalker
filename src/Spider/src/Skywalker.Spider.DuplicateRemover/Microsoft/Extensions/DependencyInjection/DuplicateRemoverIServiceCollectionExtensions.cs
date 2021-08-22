using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Spider.Abstractions;
using Skywalker.Spider.DuplicateRemover;
using Skywalker.Spider.DuplicateRemover.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class DuplicateRemoverIServiceCollectionExtensions
{
    public static ISpiderBuilder AddBloomFilterDuplicateRemover(this ISpiderBuilder builder)
    {
        builder.Services.AddSingleton<IDuplicateRemover, BloomFilterDuplicateRemover>();
        return builder;
    }

    public static ISpiderBuilder AddHashSetDuplicateRemover(this ISpiderBuilder builder)
    {
        builder.Services.AddSingleton<IDuplicateRemover, HashSetDuplicateRemover>();
        return builder;
    }

    public static ISpiderBuilder AddDuplicateRemover(this ISpiderBuilder builder)
    {
        builder.Services.TryAddSingleton<IDuplicateRemover, NoneDuplicateRemover>();
        return builder;
    }
}
