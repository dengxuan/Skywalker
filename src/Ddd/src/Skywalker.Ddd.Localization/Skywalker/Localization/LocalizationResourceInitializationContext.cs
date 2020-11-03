﻿using System;

namespace Skywalker.Localization
{
    public class LocalizationResourceInitializationContext
    {
        public LocalizationResource Resource { get; }

        public IServiceProvider ServiceProvider { get; }

        public LocalizationResourceInitializationContext(LocalizationResource resource, IServiceProvider serviceProvider)
        {
            Resource = resource;
            ServiceProvider = serviceProvider;
        }
    }
}