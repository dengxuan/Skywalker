using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Skywalker.AspNetCore.Mvc.Models;

namespace Skywalker.AspNetCore.Mvc.Response.Wrapping
{
    public class EmptyActionResultWrapper : IActionResultWrapper
    {
        public void Wrap(ResultExecutingContext actionResult)
        {
            actionResult.Result = new ObjectResult(new AjaxResponse());
        }
    }
}
