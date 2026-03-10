using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Skywalker.Ddd.AspNetCore.Mvc.Models.Abstractions;

namespace Skywalker.Ddd.AspNetCore.Mvc.Results.Wrapping;

public class ObjectActionResultWrapper : IActionResultWrapper
{
    public void Wrap(ResultExecutingContext actionResult)
    {
        if (actionResult.Result is not ObjectResult objectResult)
        {
            throw new ArgumentException($"{nameof(actionResult)} should be ObjectResult!");
        }

        if (objectResult.Value is not AjaxResponseBase)
        {
            actionResult.Result = new ObjectResult(new AjaxResponse(objectResult.Value));
        }
    }
}
