using Skywalker;
using Skywalker.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    internal static class LocalizationSkywalkerBuilderExtensions
    {
        public static SkywalkerDddBuilder AddLocalization(SkywalkerDddBuilder builder)
        {
            builder.Services.AddTransient<ILanguageProvider, DefaultLanguageProvider>();
            return builder;
        }
    }
}
