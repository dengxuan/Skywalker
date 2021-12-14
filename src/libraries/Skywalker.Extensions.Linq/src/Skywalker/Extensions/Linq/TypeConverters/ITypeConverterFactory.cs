using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Extensions.Linq.TypeConverters
{
    interface ITypeConverterFactory
    {
        /// <summary>
        /// Returns a type converter for the specified type.
        /// </summary>
        /// <param name="type">The System.Type of the target component.</param>
        /// <returns>A System.ComponentModel.TypeConverter for the specified type.</returns>
        TypeConverter GetConverter(Type type);
    }
}
