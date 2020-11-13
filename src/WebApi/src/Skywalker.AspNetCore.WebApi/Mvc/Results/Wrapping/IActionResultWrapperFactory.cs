using Microsoft.AspNetCore.Mvc.Filters;

namespace Skywalker.AspNetCore.WebApi.Mvc.Results.Wrapping
{

    public interface IActionResultWrapperFactory
    {
        IActionResultWrapper CreateFor(ResultExecutingContext actionResult);
    }
}
