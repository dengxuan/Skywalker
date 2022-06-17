// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Skywalker.Ddd.EntityFrameworkCore.ValueConverters;

public class SkywalkerArrayValueConverter : ValueConverter<string[], string>
{
    private const string SEPARATOR = ",";

    private static string To(string[] strings)
    {
        return strings.Where(predicate => !predicate.IsNullOrWhiteSpace()).JoinAsString(SEPARATOR);
    }

    private static string[] From(string s)
    {
        return s.Split(SEPARATOR,StringSplitOptions.RemoveEmptyEntries);
    }
    
    public SkywalkerArrayValueConverter() : base(x => To(x), x => From(x))
    {
    }
}
