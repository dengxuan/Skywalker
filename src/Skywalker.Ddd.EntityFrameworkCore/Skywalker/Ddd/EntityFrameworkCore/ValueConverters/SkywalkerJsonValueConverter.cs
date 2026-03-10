using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
#if NET5_0_OR_GREATER
using System.Text.Json;
#else
using Newtonsoft.Json;
#endif

namespace Skywalker.Ddd.EntityFrameworkCore.ValueConverters;

public class SkywalkerJsonValueConverter<TPropertyType> : ValueConverter<TPropertyType, string>
{
    public SkywalkerJsonValueConverter() : base(d => SerializeObject(d), s => DeserializeObject(s)) { }

    private static string SerializeObject(TPropertyType d)
    {
#if NET5_0_OR_GREATER
        return JsonSerializer.Serialize(d);
#else
        return JsonConvert.SerializeObject(d);
#endif
    }

    private static TPropertyType DeserializeObject(string s)
    {
#if NET5_0_OR_GREATER
        return JsonSerializer.Deserialize<TPropertyType>(s)!;
#else
        return JsonConvert.DeserializeObject<TPropertyType>(s)!;
#endif
    }
}
