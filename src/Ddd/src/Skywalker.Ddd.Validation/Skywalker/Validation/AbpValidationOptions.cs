﻿using System;
using System.Collections.Generic;
using Skywalker.Collections;
using Skywalker.Collections.Generic;

namespace Skywalker.Validation
{
    public class AbpValidationOptions
    {
        public List<Type> IgnoredTypes { get; }

        public ITypeList<IObjectValidationContributor> ObjectValidationContributors { get; set; }

        public AbpValidationOptions()
        {
            IgnoredTypes = new List<Type>();
            ObjectValidationContributors = new TypeList<IObjectValidationContributor>();
        }
    }
}