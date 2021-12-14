using Microsoft.EntityFrameworkCore;
using Skywalker.Domain.Entities;
using Skywalker.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    internal static class DbContextHelper
    {
        public static IEnumerable<Type> GetEntityTypes<TDbContext>() where TDbContext : DbContext
        {
            return from property in typeof(TDbContext).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                   where ReflectionHelper.IsAssignableToGenericType(property.PropertyType, typeof(DbSet<>))
                   where typeof(IEntity).IsAssignableFrom(property.PropertyType.GenericTypeArguments[0])
                   select property.PropertyType.GenericTypeArguments[0];
        }
    }
}