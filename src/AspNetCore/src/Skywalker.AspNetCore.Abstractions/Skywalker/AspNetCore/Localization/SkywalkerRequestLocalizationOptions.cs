using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Skywalker.AspNetCore.Localization
{
    public class SkywalkerRequestLocalizationOptions
    {
        public List<Func<IServiceProvider, RequestLocalizationOptions, Task>> RequestLocalizationOptionConfigurators { get; }

        public SkywalkerRequestLocalizationOptions()
        {
            RequestLocalizationOptionConfigurators = new List<Func<IServiceProvider, RequestLocalizationOptions, Task>>();
        }
    }
}
