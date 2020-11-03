﻿using System.Diagnostics.CodeAnalysis;

namespace Skywalker.EntityFrameworkCore.Modeling
{
    public class SkywalkerModelBuilderConfigurationOptions
    {
        [NotNull]
        public string TablePrefix { get; set; }

        [MaybeNull]
        public string? Schema { get; set; }

        public SkywalkerModelBuilderConfigurationOptions([NotNull] string tablePrefix = "", [MaybeNull] string? schema = null)
        {
            Check.NotNull(tablePrefix, nameof(tablePrefix), $"{nameof(tablePrefix)} can not be null! Set to empty string if you don't want a table prefix.");

            TablePrefix = tablePrefix;
            Schema = schema;
        }
    }
}
