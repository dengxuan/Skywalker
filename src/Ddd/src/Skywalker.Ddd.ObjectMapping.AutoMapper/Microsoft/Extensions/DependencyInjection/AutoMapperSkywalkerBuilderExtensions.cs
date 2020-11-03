using AutoMapper;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Skywalker;
using Skywalker.Ddd.ObjectMapping.AutoMapper;
using Skywalker.Ddd.ObjectMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AutoMapperSkywalkerBuilderExtensions
    {
        public static SkywalkerBuilder AddAutoMapper(this SkywalkerBuilder builder, Action<SkywalkerAutoMapperOptions> optionsBuilder)
        {
            builder.Services.Configure(optionsBuilder);
            builder.Services.TryAddSingleton(SkywalkerAutoMapperOptionsFactory.Create);

            return builder;
        }
    }
}
