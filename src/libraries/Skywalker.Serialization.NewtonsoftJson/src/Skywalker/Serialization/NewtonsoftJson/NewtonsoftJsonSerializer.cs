using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Skywalker.Serialization.Abstractions;

namespace Skywalker.Serialization.NewtonsoftJson;

public class NewtonsoftJsonSerializer : ISerializer
{
    private readonly NewtonsoftJsonIsoDateTimeConverter _dateTimeConverter;

    private static readonly CamelCaseExceptDictionaryKeysResolver s_sharedCamelCaseExceptDictionaryKeysResolver = new();

    public NewtonsoftJsonSerializer(NewtonsoftJsonIsoDateTimeConverter dateTimeConverter)
    {
        _dateTimeConverter = dateTimeConverter;
    }

    public string Serialize(object @object, bool camelCase = true, bool indented = false)
    {
        return JsonConvert.SerializeObject(@object, CreateSerializerSettings(camelCase, indented));
    }

    public T? Deserialize<T>(byte[] bytes, bool camelCase = true)
    {
        var jsonString = Encoding.UTF8.GetString(bytes);
        return JsonConvert.DeserializeObject<T>(jsonString, CreateSerializerSettings(camelCase));
    }

    public object? Deserialize(Type type, byte[] bytes, bool camelCase = true)
    {
        var jsonString = Encoding.UTF8.GetString(bytes);
        return JsonConvert.DeserializeObject(jsonString, type, CreateSerializerSettings(camelCase));
    }

    protected virtual JsonSerializerSettings CreateSerializerSettings(bool camelCase = true, bool indented = false)
    {
        var settings = new JsonSerializerSettings();

        settings.Converters.Insert(0, _dateTimeConverter);

        if (camelCase)
        {
            settings.ContractResolver = s_sharedCamelCaseExceptDictionaryKeysResolver;
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
