using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Skywalker.Extensions.Linq.Parser;

internal static class ConstantExpressionHelper
{
    private static readonly ConcurrentDictionary<object, Expression> s_expressions = new();
    private static readonly ConcurrentDictionary<Expression, string> s_literals = new();

    public static bool TryGetText(Expression expression, out string? text)
    {
        return s_literals.TryGetValue(expression, out text);
    }

    public static Expression CreateLiteral(object value, string text)
    {
        if (!s_expressions.ContainsKey(value))
        {
            var constantExpression = Expression.Constant(value);

            s_expressions.TryAdd(value, constantExpression);
            s_literals.TryAdd(constantExpression, text);
        }

        return s_expressions[value];
    }
}
