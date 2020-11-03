using Microsoft.EntityFrameworkCore;
using Simple.Domain.Users;
using Skywalker;
using Skywalker.EntityFrameworkCore.Modeling;
using System;

namespace Simple.EntityFrameworkCore.Weixin.EntityFrameworkCore
{
    public static class SimpleDbContextModelCreatingExtensions
    {

        public static void ConfigureWeixin(this ModelBuilder builder, Action<SimpleModelBuilderConfigurationOptions>? optionsAction = null)
        {

            Check.NotNull(builder, nameof(builder));

            var options = new SimpleModelBuilderConfigurationOptions();

            optionsAction?.Invoke(options);

            builder.Entity<User>(b =>
            {
                b.ToTable(options.TablePrefix + "Users", options.Schema);

                b.ConfigureByConvention();

                b.Property(x => x.Name).IsRequired();

            });
        }
    }
}
