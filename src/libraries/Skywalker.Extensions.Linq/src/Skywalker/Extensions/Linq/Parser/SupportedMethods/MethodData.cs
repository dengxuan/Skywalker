using System.Linq.Expressions;
using System.Reflection;

namespace Skywalker.Extensions.Linq.Parser.SupportedMethods
{
    internal class MethodData
    {
        public MethodBase MethodBase { get; set; }
        public ParameterInfo[] Parameters { get; set; }
        public Expression[] Args { get; set; }
    }
}
