using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Ddd.EntityFrameworkCore.Modeling
{
    public class SkywalkerModelBuilderConfigurationOptions
    {
        
        public string TablePrefix { get; set; }

        
        public string? Schema { get; set; }

        public SkywalkerModelBuilderConfigurationOptions(string tablePrefix = "",  string? schema = null)
        {
            tablePrefix.NotNull(nameof(tablePrefix), $"{nameof(tablePrefix)} can not be null! Set to empty string if you don't want a table prefix.");

            TablePrefix = tablePrefix;
            Schema = schema;
        }
    }
}
