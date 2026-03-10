using Microsoft.AspNetCore.Mvc.Filters;

namespace Skywalker.Ddd.AspNetCore.Mvc.Results.Wrapping;


public interface IActionResultWrapperFactory
{
    IActionResultWrapper CreateFor(ResultExecutingContext actionResult);
}
