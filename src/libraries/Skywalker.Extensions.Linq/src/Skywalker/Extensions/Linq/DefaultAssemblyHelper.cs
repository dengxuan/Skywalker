using System;
using System.Reflection;

namespace Skywalker.Extensions.Linq
{
    internal class DefaultAssemblyHelper : IAssemblyHelper
    {
        public Assembly[] GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}
