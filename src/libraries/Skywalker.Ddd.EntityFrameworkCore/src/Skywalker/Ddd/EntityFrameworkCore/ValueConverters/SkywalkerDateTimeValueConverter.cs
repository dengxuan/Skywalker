using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Skywalker.Extensions.Timing;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Ddd.EntityFrameworkCore.ValueConverters
{
    public class SkywalkerDateTimeValueConverter : ValueConverter<DateTime?, DateTime?>
    {
        public SkywalkerDateTimeValueConverter(IClock clock,  ConverterMappingHints? mappingHints = null)
            : base(
                x => x.HasValue ? clock.Normalize(x.Value) : x,
                x => x.HasValue ? clock.Normalize(x.Value) : x, mappingHints)
        {
        }
    }
}