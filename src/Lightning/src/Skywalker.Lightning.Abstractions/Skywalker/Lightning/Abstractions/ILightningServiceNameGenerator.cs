using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Skywalker.Lightning.Abstractions
{
    public interface ILightningServiceNameGenerator
    {
        string GetLightningServiceName(MethodInfo methodInfo, ParameterInfo[] parameterInfos);
    }
}
