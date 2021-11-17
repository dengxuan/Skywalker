using Simple.Domain;
using Skywalker.Ddd.EntityFrameworkCore.Modeling;
using System.Diagnostics.CodeAnalysis;

namespace Simple.Infrastructure.EntityFrameworkCore
{
    public class SimpleModelBuilderConfigurationOptions : SkywalkerModelBuilderConfigurationOptions
    {
        public SimpleModelBuilderConfigurationOptions([NotNull] string tablePrefix = SimpleConsts.DefaultDbTablePrefix, [MaybeNull] string? schema = SimpleConsts.DefaultDbSchema) : base(tablePrefix, schema)
        {

        }
    }
}
