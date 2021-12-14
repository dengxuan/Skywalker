using System.Linq.Expressions;

namespace Skywalker.Extensions.Linq
{
    internal class DynamicOrdering
    {
        public Expression Selector;
        public bool Ascending;
        public string MethodName;
    }
}