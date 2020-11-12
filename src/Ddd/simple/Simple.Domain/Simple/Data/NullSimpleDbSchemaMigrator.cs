using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Data
{
    internal class NullSimpleDbSchemaMigrator : ISimpleDbSchemaMigrator
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}
