using Skywalker.Ddd.Infrastructure.Abstractions;
using Skywalker.Domain.Entities;
using Skywalker.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Skywalker.EntityFrameworkCore
{
    internal static class DbContextHelper
    {
        public static IEnumerable<Type> GetEntityTypes(Type dbContextType)
        {
            return
                from property in dbContextType.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where
                    ReflectionHelper.IsAssignableToGenericType(property.PropertyType, typeof(IDataCollection<>)) &&
                    typeof(IEntity).IsAssignableFrom(property.PropertyType.GenericTypeArguments[0])
                select property.PropertyType.GenericTypeArguments[0];
        }
    }
}