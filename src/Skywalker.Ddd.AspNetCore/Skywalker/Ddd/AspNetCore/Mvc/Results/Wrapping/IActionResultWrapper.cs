using Microsoft.AspNetCore.Mvc.Filters;

namespace Skywalker.Ddd.AspNetCore.Mvc.Results.Wrapping;

public interface IActionResultWrapper
{
    void Wrap(ResultExecutingContext actionResult);
}
