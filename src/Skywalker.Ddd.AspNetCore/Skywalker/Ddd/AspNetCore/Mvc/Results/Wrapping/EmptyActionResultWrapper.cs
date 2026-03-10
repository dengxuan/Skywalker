using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Skywalker.Ddd.AspNetCore.Mvc.Models.Abstractions;

namespace Skywalker.Ddd.AspNetCore.Mvc.Results.Wrapping;

public class EmptyActionResultWrapper : IActionResultWrapper
{
    public void Wrap(ResultExecutingContext actionResult)
    {
        actionResult.Result = new ObjectResult(new AjaxResponse());
    }
}
