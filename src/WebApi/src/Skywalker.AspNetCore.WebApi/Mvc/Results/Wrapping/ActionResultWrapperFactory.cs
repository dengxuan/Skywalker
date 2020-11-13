using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Skywalker.AspNetCore.WebApi.Mvc.Results.Wrapping
{
    public class ActionResultWrapperFactory : IActionResultWrapperFactory
    {
        public IActionResultWrapper CreateFor(ResultExecutingContext actionResult)
        {
            if(actionResult == null)
            {
                throw new ArgumentNullException(nameof(actionResult));
            }

            if (actionResult.Result is ObjectResult)
            {
                return new ObjectActionResultWrapper();
            }

            if (actionResult.Result is JsonResult)
            {
                return new JsonActionResultWrapper();
            }

            if (actionResult.Result is EmptyResult)
            {
                return new EmptyActionResultWrapper();
            }

            return new NullActionResultWrapper();
        }
    }
}
