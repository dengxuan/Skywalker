using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Transfer.Domain;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Transfer.EntityFrameworkCore
{
    public class TransferModelBuilderConfigurationOptions : SkywalkerModelBuilderConfigurationOptions
    {
        public TransferModelBuilderConfigurationOptions([NotNull] string tablePrefix = TransferConsts.DefaultDbTablePrefix, [MaybeNull] string? schema = TransferConsts.DefaultDbSchema) : base(tablePrefix, schema)
        {

        }
    }
}
