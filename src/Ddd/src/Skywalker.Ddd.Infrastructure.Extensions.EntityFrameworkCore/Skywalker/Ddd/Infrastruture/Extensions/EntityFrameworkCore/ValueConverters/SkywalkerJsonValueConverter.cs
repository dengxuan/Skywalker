using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace Skywalker.EntityFrameworkCore.ValueConverters
{
    public class SkywalkerJsonValueConverter<TPropertyType> : ValueConverter<TPropertyType, string>
    {
        public SkywalkerJsonValueConverter()
            : base(
                d => SerializeObject(d),
                s => DeserializeObject(s))
        {

        }

        private static string SerializeObject(TPropertyType d)
        {
            return JsonConvert.SerializeObject(d, Formatting.None);
        }

        private static TPropertyType DeserializeObject(string s)
        {
            return JsonConvert.DeserializeObject<TPropertyType>(s);
        }
    }
}
