using Microsoft.AspNetCore.Mvc.Filters;

namespace Skywalker.AspNetCore.WebApi.Mvc.Results.Wrapping
{
    public class NullActionResultWrapper : IActionResultWrapper
    {
        public void Wrap(ResultExecutingContext actionResult)
        {

        }
    }
}
