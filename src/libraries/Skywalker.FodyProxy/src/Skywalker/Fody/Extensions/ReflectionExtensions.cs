using Mono.Cecil;
using System.Reflection;

namespace Skywalker.Fody.Extensions;

internal static class ReflectionExtensions
{
    public static MethodReference ImportInto(this MethodBase methodInfo, ModuleDefinition moduleDef) => moduleDef.ImportReference(methodInfo);

}
