using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Skywalker.Extensions.Linq.Parser
{
    interface ITypeFinder
    {
        Type FindTypeByName([NotNull] string name, [MaybeNull] ParameterExpression[] expressions, bool forceUseCustomTypeProvider);
    }
}
