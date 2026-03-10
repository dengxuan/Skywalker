using System.Linq.Expressions;
using System.Reflection;

namespace Skywalker.Extensions.Linq.Parser;

/// <summary>
/// ExpressionPromoter
/// </summary>
public class ExpressionPromoter : IExpressionPromoter
{
    private readonly NumberParser _numberParser;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionPromoter"/> class.
    /// </summary>
    /// <param name="config">The ParsingConfig.</param>
    public ExpressionPromoter(ParsingConfig config)
    {
        _numberParser = new NumberParser(config);
    }

    /// <inheritdoc cref="IExpressionPromoter.Promote(Expression, Type?, bool, bool)"/>
    public virtual Expression? Promote(Expression expr, Type? type, bool exact, bool convertExpr)
    {
        if (expr.Type == type)
        {
            return expr;
        }

        if (expr is ConstantExpression ce)
        {
            if (Constants.IsNull(ce))
            {
                if (!type?.GetTypeInfo().IsValueType == true || TypeHelper.IsNullableType(type!))
                {
                    return Expression.Constant(null, type!);
                }
            }
            else
            {
                if (ConstantExpressionHelper.TryGetText(ce, out var text))
                {
                    var target = TypeHelper.GetNonNullableType(type!);
                    object? value = null;

                    switch (Type.GetTypeCode(ce.Type))
                    {
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                            value = _numberParser.ParseNumber(text!, target);

                            // Make sure an enum value stays an enum value
                            if (target.IsEnum)
                            {
                                value = Enum.ToObject(target, value!);
                            }
                            break;

                        case TypeCode.Double:
                            if (target == typeof(decimal) || target == typeof(double)) value = _numberParser.ParseNumber(text!, target);
                            break;

                        case TypeCode.String:
                            value = TypeHelper.ParseEnum(text!, target);
                            break;
                    }
                    if (value != null)
                    {
                        return Expression.Constant(value, type!);
                    }
                }
            }
        }

        if (TypeHelper.IsCompatibleWith(expr.Type, type!))
        {
            if (type?.GetTypeInfo().IsValueType == true || exact || expr.Type.GetTypeInfo().IsValueType && convertExpr)
            {
                return Expression.Convert(expr, type!);
            }

            return expr;
        }

        return null;
    }
}
