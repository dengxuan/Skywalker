using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Skywalker.Ddd.EntityFrameworkCore.ValueConverters;

public class SkywalkerJsonValueConverter<TPropertyType> : ValueConverter<TPropertyType, string>
{
    public SkywalkerJsonValueConverter() : base(d => SerializeObject(d), s => DeserializeObject(s)) { }

    private static string SerializeObject(TPropertyType d)
    {
        return JsonSerializer.Serialize(d);
    }

    private static TPropertyType DeserializeObject(string s)
    {
        return JsonSerializer.Deserialize<TPropertyType>(s)!;
    }
}
