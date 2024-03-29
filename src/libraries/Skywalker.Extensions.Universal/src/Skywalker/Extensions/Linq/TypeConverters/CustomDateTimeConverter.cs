﻿using System.ComponentModel;
using System.Globalization;

namespace Skywalker.Extensions.Linq.TypeConverters;

internal class CustomDateTimeConverter : DateTimeOffsetConverter
{
    /// <summary>
    /// Converts the specified object to a <see cref="DateTime"></see>.
    /// </summary>
    /// <param name="context">The date format context.</param>
    /// <param name="culture">The date culture.</param>
    /// <param name="value">The object to be converted.</param>
    /// <returns>A <see cref="Nullable{DateTime}"></see> that represents the specified object.</returns>
    /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
#if NET6_0_OR_GREATER
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        var dateTimeOffset = base.ConvertFrom(context, culture, value) as DateTimeOffset?;

        return dateTimeOffset?.UtcDateTime;
    }
#elif NET5_0_OR_GREATER || NETSTANDARD
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        var dateTimeOffset = base.ConvertFrom(context, culture, value) as DateTimeOffset?;

        return dateTimeOffset?.UtcDateTime!;
    }
#endif
}
