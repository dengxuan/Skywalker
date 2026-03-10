using System.Linq.Expressions;

namespace Skywalker.Extensions.Linq;

internal class DynamicOrdering
{
    public Expression Selector;
    public bool Ascending;
    public string MethodName;

    internal DynamicOrdering(Expression selector, bool ascending, string methodName)
    {
        Selector = selector;
        Ascending = ascending;
        MethodName = methodName;
    }
}
