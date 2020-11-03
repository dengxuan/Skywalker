using System;

namespace Skywalker.Localization
{
    public interface IInheritedResourceTypesProvider
    {
        Type[] GetInheritedResourceTypes();
    }
}