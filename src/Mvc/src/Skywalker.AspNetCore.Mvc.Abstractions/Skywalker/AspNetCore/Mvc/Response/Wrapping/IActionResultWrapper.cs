using Microsoft.AspNetCore.Mvc.Filters;

namespace Skywalker.AspNetCore.Mvc.Response.Wrapping
{
    public interface IActionResultWrapper
    {
        void Wrap(ResultExecutingContext actionResult);
    }
}
