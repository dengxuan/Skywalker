using Microsoft.AspNetCore.Mvc;

namespace Skywalker.Ddd.AspNetCore.Mvc.Results;

public static class ActionResultHelper
{
    public static bool IsObjectResult(Type returnType)
    {
        //Get the actual return type (unwrap Task)
        if (returnType == typeof(Task) || returnType == typeof(ValueTask))
        {
            returnType = typeof(void);
        }
        else if (returnType.IsGenericType)
        {
            var genericTypeDefinition = returnType.GetGenericTypeDefinition();
            if(genericTypeDefinition == typeof(Task<>) || genericTypeDefinition == typeof(ValueTask<>))
            {
                returnType = returnType.GenericTypeArguments[0];
            }
        }

        if (typeof(ObjectResult).IsAssignableFrom(returnType))
        {
            return true;
        }

        return true;
    }
}
