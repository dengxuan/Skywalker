using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Skywalker.AspNetCore.WebApi.Models;
using System;

namespace Skywalker.AspNetCore.WebApi.Mvc.Results.Wrapping
{
    public class ObjectActionResultWrapper : IActionResultWrapper
    {
        public void Wrap(ResultExecutingContext actionResult)
        {
            var objectResult = actionResult.Result as ObjectResult;
            if (objectResult == null)
            {
                throw new ArgumentException($"{nameof(actionResult)} should be ObjectResult!");
            }

            if (!(objectResult.Value is AjaxResponseBase))
            {
                actionResult.Result = new ObjectResult(new AjaxResponse(objectResult.Value));
            }
        }
    }
}
