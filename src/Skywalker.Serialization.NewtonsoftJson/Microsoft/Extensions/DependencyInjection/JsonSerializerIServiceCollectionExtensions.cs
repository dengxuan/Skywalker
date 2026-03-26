// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Newtonsoft.Json.Converters;
using Skywalker.Serialization.NewtonsoftJson;


namespace Microsoft.Extensions.DependencyInjection;

public static class JsonSerializerIServiceCollectionExtensions
{
    public static IServiceCollection AddJsonSerializer(this IServiceCollection services)
    {
        services.AddTimezone();
        SkywalkerSerializationNewtonsoftJsonAutoServiceExtensions.AddAutoServices(services);
        services.AddSingleton<IsoDateTimeConverter>(sp => sp.GetRequiredService<NewtonsoftJsonIsoDateTimeConverter>());
        return services;
    }
}
