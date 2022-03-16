using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Skywalker.Extensions.Timezone;

namespace Skywalker.Serialization.NewtonsoftJson;

public class NewtonsoftJsonIsoDateTimeConverter : IsoDateTimeConverter
{
    private readonly IClock _clock;

    public NewtonsoftJsonIsoDateTimeConverter(IClock clock, IOptions<SerializationOptions> serializationOptions)
    {
        _clock = clock;

        if (serializationOptions.Value.DefaultDateTimeFormat != null)
        {
            DateTimeFormat = serializationOptions.Value.DefaultDateTimeFormat;
        }
    }

    public override bool CanConvert(Type objectType)
    {
        if (objectType == typeof(DateTime) || objectType == typeof(DateTime?))
        {
            return true;
        }

        return false;
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var date = base.ReadJson(reader, objectType, existingValue, serializer) as DateTime?;

        if (date.HasValue)
        {
            return _clock.Normalize(date.Value);
        }

        return null;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        var date = value as DateTime?;
        base.WriteJson(writer, date.HasValue ? _clock.Normalize(date.Value) : value, serializer);
    }
}
