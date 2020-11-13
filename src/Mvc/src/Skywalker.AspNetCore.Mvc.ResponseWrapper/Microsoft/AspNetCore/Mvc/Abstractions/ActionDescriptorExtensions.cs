using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Reflection;

namespace Microsoft.AspNetCore.Mvc.Abstractions
{
    internal static class ActionDescriptorExtensions
    {
        internal static ControllerActionDescriptor AsControllerActionDescriptor(this ActionDescriptor actionDescriptor)
        {
            if (actionDescriptor is not ControllerActionDescriptor controllerActionDescriptor)
            {
                throw new Exception($"{nameof(actionDescriptor)} should be type of {typeof(ControllerActionDescriptor).AssemblyQualifiedName}");
            }

            return controllerActionDescriptor;
        }

        internal static MethodInfo GetMethodInfo(this ActionDescriptor actionDescriptor)
        {
            return actionDescriptor.AsControllerActionDescriptor().MethodInfo;
        }
    }
}
