using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Skywalker.AspNetCore.Mvc.Abstractions.Models;
using System;

namespace Skywalker.AspNetCore.Mvc.Response.Wrapping
{
    public class ObjectActionResultWrapper : IActionResultWrapper
    {
        public void Wrap(ResultExecutingContext actionResult)
        {
            if (actionResult.Result is not ObjectResult objectResult)
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
