using Simple.Domain;
using Skywalker.Ddd.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Simple.EntityFrameworkCore
{
    public class SimpleModelBuilderConfigurationOptions : SkywalkerModelBuilderConfigurationOptions
    {
        public SimpleModelBuilderConfigurationOptions([NotNull] string tablePrefix = SimpleConsts.DefaultDbTablePrefix,[MaybeNull] string? schema = SimpleConsts.DefaultDbSchema) : base(tablePrefix, schema)
        {

        }
    }
}
