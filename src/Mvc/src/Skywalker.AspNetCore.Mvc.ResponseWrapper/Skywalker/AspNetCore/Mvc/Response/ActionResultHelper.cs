using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Skywalker.AspNetCore.Mvc.Response
{
    public static class ActionResultHelper
    {
        public static bool IsObjectResult(Type returnType)
        {
            //Get the actual return type (unwrap Task)
            if (returnType == typeof(Task))
            {
                returnType = typeof(void);
            }
            else if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                returnType = returnType.GenericTypeArguments[0];
            }

            if (typeof(ObjectResult).IsAssignableFrom(returnType))
            {
                return true;
            }

            return true;
        }
    }
}
