using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Skywalker.Ddd.AspNetCore.Mvc.Models.Abstractions;

namespace Skywalker.Ddd.AspNetCore.Mvc.Results.Wrapping;

public class BadRequestResultWrapper : IActionResultWrapper
{
    public void Wrap(ResultExecutingContext actionResult)
    {
        if (actionResult.Result is not BadRequestResult badRequestResult)
        {
            throw new ArgumentException($"{nameof(actionResult)} should be BadRequestObjectResult!");
        }
        actionResult.Result = new ObjectResult(new AjaxResponse(new Error(badRequestResult.StatusCode.ToString(), "")));
    }
}
