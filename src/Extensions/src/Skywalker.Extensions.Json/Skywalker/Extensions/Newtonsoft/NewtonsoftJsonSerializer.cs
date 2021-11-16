using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Skywalker.Extensions.Newtonsoft
{
    public class NewtonsoftJsonSerializer : IJsonSerializer
    {
        private readonly SkywalkerJsonIsoDateTimeConverter _dateTimeConverter;

        private static readonly CamelCaseExceptDictionaryKeysResolver SharedCamelCaseExceptDictionaryKeysResolver =
            new CamelCaseExceptDictionaryKeysResolver();

        public NewtonsoftJsonSerializer(SkywalkerJsonIsoDateTimeConverter dateTimeConverter)
        {
            _dateTimeConverter = dateTimeConverter;
        }

        public string Serialize(object obj, bool camelCase = true, bool indented = false)
        {
            return JsonConvert.SerializeObject(obj, CreateSerializerSettings(camelCase, indented));
        }

        public T? Deserialize<T>(string jsonString, bool camelCase = true)
        {
            return JsonConvert.DeserializeObject<T>(jsonString, CreateSerializerSettings(camelCase));
        }

        public object? Deserialize(Type type, string jsonString, bool camelCase = true)
        {
            return JsonConvert.DeserializeObject(jsonString, type, CreateSerializerSettings(camelCase));
        }

        protected virtual JsonSerializerSettings CreateSerializerSettings(bool camelCase = true, bool indented = false)
        {
            var settings = new JsonSerializerSettings();

            settings.Converters.Insert(0, _dateTimeConverter);

            if (camelCase)
            {
                settings.ContractResolver = SharedCamelCaseExceptDictionaryKeysResolver;
            }

            if (indented)
            {
                settings.Formatting = Formatting.Indented;
            }

            return settings;
        }

        private class CamelCaseExceptDictionaryKeysResolver : CamelCasePropertyNamesContractResolver
        {
            protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
            {
                var contract = base.CreateDictionaryContract(objectType);

                contract.DictionaryKeyResolver = propertyName => propertyName;

                return contract;
            }
        }
    }
}