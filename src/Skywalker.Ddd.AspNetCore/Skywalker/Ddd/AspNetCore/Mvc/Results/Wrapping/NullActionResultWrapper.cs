using Microsoft.AspNetCore.Mvc.Filters;

namespace Skywalker.Ddd.AspNetCore.Mvc.Results.Wrapping;

public class NullActionResultWrapper : IActionResultWrapper
{
    public void Wrap(ResultExecutingContext actionResult)
    {

    }
}
