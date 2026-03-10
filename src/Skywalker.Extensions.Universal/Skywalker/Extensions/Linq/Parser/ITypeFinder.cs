using System.Linq.Expressions;

namespace Skywalker.Extensions.Linq.Parser;

interface ITypeFinder
{
    Type? FindTypeByName(string name, ParameterExpression?[]? expressions, bool forceUseCustomTypeProvider);
}
