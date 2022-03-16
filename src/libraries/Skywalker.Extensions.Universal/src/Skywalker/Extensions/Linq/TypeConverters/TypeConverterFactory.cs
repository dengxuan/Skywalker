using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Skywalker.Extensions.Linq.Parser;

namespace Skywalker.Extensions.Linq.TypeConverters
{
    internal class TypeConverterFactory : ITypeConverterFactory
    {
        private readonly ParsingConfig _config;

        public TypeConverterFactory(ParsingConfig config)
        {
            Check.NotNull(config, nameof(config));

            _config = config;
        }

        /// <see cref="ITypeConverterFactory.GetConverter"/>
        public TypeConverter GetConverter(Type type)
        {
            Check.NotNull(type, nameof(type));

            if (_config.DateTimeIsParsedAsUTC && (type == typeof(DateTime) || type == typeof(DateTime?)))
            {
                return new CustomDateTimeConverter();
            }

            var typeToCheck = TypeHelper.IsNullableType(type) ? TypeHelper.GetNonNullableType(type) : type;
            if (_config.TypeConverters != null && _config.TypeConverters.TryGetValue(typeToCheck, out var typeConverter))
            {
                return typeConverter;
            }
            return TypeDescriptor.GetConverter(type);
        }
    }
}
