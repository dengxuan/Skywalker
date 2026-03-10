using System.Text.Json;

namespace Skywalker.Localization.Json;

/// <summary>
/// Builds a localization dictionary from JSON content.
/// </summary>
public static class JsonLocalizationDictionaryBuilder
{
    /// <summary>
    /// Builds a dictionary from JSON content.
    /// </summary>
    /// <param name="jsonContent">The JSON content.</param>
    /// <returns>A dictionary of localized strings.</returns>
    public static Dictionary<string, string> Build(string jsonContent)
    {
        var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(jsonContent))
        {
            return dictionary;
        }

        using var document = JsonDocument.Parse(jsonContent);
        ProcessElement(document.RootElement, dictionary, prefix: null);

        return dictionary;
    }

    private static void ProcessElement(JsonElement element, Dictionary<string, string> dictionary, string? prefix)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    // Skip special properties like "culture"
                    if (property.Name.Equals("culture", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    // Skip "texts" wrapper and process its children directly
                    if (property.Name.Equals("texts", StringComparison.OrdinalIgnoreCase) &&
                        property.Value.ValueKind == JsonValueKind.Object)
                    {
                        ProcessElement(property.Value, dictionary, prefix);
                        continue;
                    }

                    var key = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}:{property.Name}";
                    ProcessElement(property.Value, dictionary, key);
                }
                break;

            case JsonValueKind.String:
                if (!string.IsNullOrEmpty(prefix))
                {
                    dictionary[prefix] = element.GetString() ?? string.Empty;
                }
                break;

            case JsonValueKind.Array:
                if (!string.IsNullOrEmpty(prefix))
                {
                    var index = 0;
                    foreach (var item in element.EnumerateArray())
                    {
                        var key = $"{prefix}[{index}]";
                        ProcessElement(item, dictionary, key);
                        index++;
                    }
                }
                break;

            case JsonValueKind.Number:
                if (!string.IsNullOrEmpty(prefix))
                {
                    dictionary[prefix] = element.GetRawText();
                }
                break;

            case JsonValueKind.True:
            case JsonValueKind.False:
                if (!string.IsNullOrEmpty(prefix))
                {
                    dictionary[prefix] = element.GetBoolean().ToString().ToLowerInvariant();
                }
                break;
        }
    }
}

