using Microsoft.AspNetCore.Mvc.Filters;

namespace Skywalker.AspNetCore.Mvc.Response.Wrapping
{

    public interface IActionResultWrapperFactory
    {
        IActionResultWrapper CreateFor(ResultExecutingContext actionResult);
    }
}
