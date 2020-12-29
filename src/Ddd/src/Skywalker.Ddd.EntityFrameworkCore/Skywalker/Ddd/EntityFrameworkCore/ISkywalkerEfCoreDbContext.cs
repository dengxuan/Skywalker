using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skywalker.Ddd.EntityFrameworkCore
{
    public interface ISkywalkerEfCoreDbContext: ISkywalkerDbContext
    {
        void Initialize(SkywalkerEfCoreDbContextInitializationContext contextInitializationContext);
    }
}
