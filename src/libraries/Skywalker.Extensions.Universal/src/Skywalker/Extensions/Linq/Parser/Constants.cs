using System.Linq.Expressions;

namespace Skywalker.Extensions.Linq.Parser;

internal static class Constants
{
    public static bool IsNull(Expression exp)
    {
        return exp is ConstantExpression cExp && cExp.Value == null;
    }
}
