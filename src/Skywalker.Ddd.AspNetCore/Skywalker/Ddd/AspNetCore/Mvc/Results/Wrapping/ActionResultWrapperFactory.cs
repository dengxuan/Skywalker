using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Skywalker.Ddd.AspNetCore.Mvc.Results.Wrapping;

public class ActionResultWrapperFactory : IActionResultWrapperFactory
{
    public IActionResultWrapper CreateFor(ResultExecutingContext actionResult)
    {
        ArgumentNullException.ThrowIfNull(actionResult);

        if (actionResult.Result is BadRequestResult)
        {
            return new BadRequestResultWrapper();
        }

        if (actionResult.Result is BadRequestObjectResult)
        {
            return new BadRequestObjectResultWrapper();
        }

        if (actionResult.Result is EmptyResult)
        {
            return new EmptyActionResultWrapper();
        }

        if (actionResult.Result is ObjectResult)
        {
            return new ObjectActionResultWrapper();
        }

        return new NullActionResultWrapper();
    }
}
