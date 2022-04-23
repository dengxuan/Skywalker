using System.Linq.Expressions;

namespace Skywalker.Extensions.Linq.Parser;

internal interface IConstantExpressionWrapper
{
    void Wrap(ref Expression expression);
}
