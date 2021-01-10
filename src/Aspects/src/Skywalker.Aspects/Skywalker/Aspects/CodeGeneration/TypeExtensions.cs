using System;

namespace Skywalker.Aspects
{
    internal static class TypeExtensions
    {       
        public static Type GetNonByRefType(this Type type)=> type.IsByRef ? type.GetElementType() : type;
    }
} 