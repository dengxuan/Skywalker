using System.Collections.Generic;
using System.Linq;

namespace Skywalker.Localization
{
    public static class SkywalkerLocalizationOptionsExtensions
    {
        public static SkywalkerLocalizationOptions AddLanguagesMapOrUpdate(this SkywalkerLocalizationOptions localizationOptions,
            string packageName, params NameValue[] maps)
        {
            foreach (var map in maps)
            {
                AddOrUpdate(localizationOptions.LanguagesMap, packageName, map);
            }

            return localizationOptions;
        }

        public static string GetLanguagesMap(this SkywalkerLocalizationOptions localizationOptions, string packageName,
            string language)
        {
            return localizationOptions.LanguagesMap.TryGetValue(packageName, out var maps)
                ? maps.FirstOrDefault(x => x.Name == language)?.Value ?? language
                : language;
        }

        public static SkywalkerLocalizationOptions AddLanguageFilesMapOrUpdate(this SkywalkerLocalizationOptions localizationOptions,
            string packageName, params NameValue[] maps)
        {
            foreach (var map in maps)
            {
                AddOrUpdate(localizationOptions.LanguageFilesMap, packageName, map);
            }

            return localizationOptions;
        }

        public static string GetLanguageFilesMap(this SkywalkerLocalizationOptions localizationOptions, string packageName,
            string language)
        {
            return localizationOptions.LanguageFilesMap.TryGetValue(packageName, out var maps)
                ? maps.FirstOrDefault(x => x.Name == language)?.Value ?? language
                : language;
        }

        private static void AddOrUpdate(IDictionary<string, List<NameValue>> maps, string packageName, NameValue value)
        {
            if (maps.TryGetValue(packageName, out var existMaps))
            {
                existMaps.GetOrAdd(x => x.Name == value.Name, () => value).Value = value.Value;
            }
            else
            {
                maps.Add(packageName, new List<NameValue> {value});
            }
        }
    }
}
