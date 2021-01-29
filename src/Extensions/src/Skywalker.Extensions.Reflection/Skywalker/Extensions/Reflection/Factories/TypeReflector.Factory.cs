﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Skywalker.Extensions.Reflection.Internals;

namespace Skywalker.Extensions.Reflection
{
    public partial class TypeReflector
    {
        internal static TypeReflector Create(TypeInfo typeInfo)
        {
            if (typeInfo == null)
            {
                throw new ArgumentNullException(nameof(typeInfo));
            }
            return ReflectorCacheUtils<TypeInfo, TypeReflector>.GetOrAdd(typeInfo, info => new TypeReflector(info));
        }
    }
}