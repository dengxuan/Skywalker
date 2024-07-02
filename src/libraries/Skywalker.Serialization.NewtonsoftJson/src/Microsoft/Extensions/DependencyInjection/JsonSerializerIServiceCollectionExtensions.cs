// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json.Converters;
using Skywalker.Serialization.Abstractions;
using Skywalker.Serialization.NewtonsoftJson;

namespace Microsoft.Extensions.DependencyInjection;

public static class JsonSerializerIServiceCollectionExtensions
{

    public static IServiceCollection AddJsonSerializer(this IServiceCollection services)
    {
        services.AddTimezone();
        services.TryAddSingleton<NewtonsoftJsonIsoDateTimeConverter>();
        services.TryAddSingleton<IsoDateTimeConverter, NewtonsoftJsonIsoDateTimeConverter>();
        services.TryAddSingleton<ISerializer, NewtonsoftJsonSerializer>();
        return services;
    }
}
