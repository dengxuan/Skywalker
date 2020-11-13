using Microsoft.AspNetCore.Mvc.Filters;

namespace Skywalker.AspNetCore.Mvc.Response.Wrapping
{
    public class NullActionResultWrapper : IActionResultWrapper
    {
        public void Wrap(ResultExecutingContext actionResult)
        {

        }
    }
}
