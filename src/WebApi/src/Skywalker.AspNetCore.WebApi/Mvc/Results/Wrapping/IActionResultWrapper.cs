using Microsoft.AspNetCore.Mvc.Filters;

namespace Skywalker.AspNetCore.WebApi.Mvc.Results.Wrapping
{
    public interface IActionResultWrapper
    {
        void Wrap(ResultExecutingContext actionResult);
    }
}
