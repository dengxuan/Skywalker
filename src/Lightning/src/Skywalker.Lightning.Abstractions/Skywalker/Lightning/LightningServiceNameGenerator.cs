using Skywalker.Lightning.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Skywalker.Lightning
{
    internal class LightningServiceNameGenerator : ILightningServiceNameGenerator
    {
        public string GetLightningServiceName(MethodInfo methodInfo, ParameterInfo[] parameterInfos)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendJoin("@", methodInfo.DeclaringType!.FullName, methodInfo.Name);
            if (parameterInfos.IsNullOrEmpty())
            {
                return sb.ToString();
            }
            string parameters = parameterInfos.Select(selector => selector.ParameterType.Name).JoinAsString("|");
            sb.AppendFormat("[{0}]", parameters);
            return sb.ToString();
        }
    }
}
