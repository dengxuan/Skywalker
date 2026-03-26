using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Skywalker.DependencyInjection;
using Skywalker.Extensions.Timezone;

namespace Skywalker.Serialization.SystemTextJson;

public class ClockDateTimeConverter : JsonConverter<DateTime>, ISingletonDependency
{
    private readonly IClock _clock;
    private readonly string _dateTimeFormat;

    public ClockDateTimeConverter(IClock clock, IOptions<SerializationOptions> serializationOptions)
    {
        _clock = clock;
        _dateTimeFormat = serializationOptions.Value.DefaultDateTimeFormat;
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var date = reader.GetDateTime();
        return _clock.Normalize(date);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(_clock.Normalize(value).ToString(_dateTimeFormat));
    }
}
