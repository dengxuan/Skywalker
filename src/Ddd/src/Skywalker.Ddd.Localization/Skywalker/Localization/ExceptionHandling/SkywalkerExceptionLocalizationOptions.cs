using System;
using System.Collections.Generic;

namespace Skywalker.Localization.ExceptionHandling
{
    public class SkywalkerExceptionLocalizationOptions
    {
        public Dictionary<string, Type> ErrorCodeNamespaceMappings { get; }

        public SkywalkerExceptionLocalizationOptions()
        {
            ErrorCodeNamespaceMappings = new Dictionary<string, Type>();
        }

        public void MapCodeNamespace(string errorCodeNamespace, Type type)
        {
            ErrorCodeNamespaceMappings[errorCodeNamespace] = type;
        }
    }
}
