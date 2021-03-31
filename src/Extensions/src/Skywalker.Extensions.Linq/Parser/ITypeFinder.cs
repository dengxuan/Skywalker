using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core.Parser
{
    interface ITypeFinder
    {
        Type FindTypeByName([NotNull] string name, [MaybeNull] ParameterExpression[] expressions, bool forceUseCustomTypeProvider);
    }
}
