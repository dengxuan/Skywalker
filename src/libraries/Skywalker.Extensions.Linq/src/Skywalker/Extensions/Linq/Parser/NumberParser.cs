// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Extensions.Linq.Parser;

/// <summary>
/// NumberParser
/// </summary>
public class NumberParser
{
    private readonly ParsingConfig _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="NumberParser"/> class.
    /// </summary>
    /// <param name="config">The ParsingConfig.</param>
    public NumberParser(ParsingConfig config)
    {
        _config = config;
    }

    /// <summary>
    /// Parses the number (text) into the specified type.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="type">The type.</param>
    public object? ParseNumber(string text, Type type)
    {
        try
        {
            switch (Type.GetTypeCode(TypeHelper.GetNonNullableType(type)))
            {
                case TypeCode.SByte:
                    return sbyte.Parse(text, _config.NumberParseCulture);
                case TypeCode.Byte:
                    return byte.Parse(text, _config.NumberParseCulture);
                case TypeCode.Int16:
                    return short.Parse(text, _config.NumberParseCulture);
                case TypeCode.UInt16:
                    return ushort.Parse(text, _config.NumberParseCulture);
                case TypeCode.Int32:
                    return int.Parse(text, _config.NumberParseCulture);
                case TypeCode.UInt32:
                    return uint.Parse(text, _config.NumberParseCulture);
                case TypeCode.Int64:
                    return long.Parse(text, _config.NumberParseCulture);
                case TypeCode.UInt64:
                    return ulong.Parse(text, _config.NumberParseCulture);
                case TypeCode.Single:
                    return float.Parse(text, _config.NumberParseCulture);
                case TypeCode.Double:
                    return double.Parse(text, _config.NumberParseCulture);
                case TypeCode.Decimal:
                    return decimal.Parse(text, _config.NumberParseCulture);
            }
        }
        catch
        {
            return null;
        }

        return null;
    }
}
