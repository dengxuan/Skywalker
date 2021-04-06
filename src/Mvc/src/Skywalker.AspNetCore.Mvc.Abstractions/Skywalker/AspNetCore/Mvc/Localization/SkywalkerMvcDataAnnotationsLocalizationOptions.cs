using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Skywalker.AspNetCore.Mvc.Localization
{
    public class SkywalkerMvcDataAnnotationsLocalizationOptions
    {
        public IDictionary<Assembly, Type> AssemblyResources { get; }

        public SkywalkerMvcDataAnnotationsLocalizationOptions()
        {
            AssemblyResources = new Dictionary<Assembly, Type>();
        }

        public void AddAssemblyResource(
            [NotNull] Type resourceType,
            params Assembly[] assemblies)
        {
            if (assemblies.IsNullOrEmpty())
            {
                assemblies = new[] { resourceType.Assembly };
            }

            foreach (var assembly in assemblies)
            {
                AssemblyResources[assembly] = resourceType;
            }
        }
    }
}