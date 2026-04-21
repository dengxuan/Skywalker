using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Skywalker.Ddd.AspNetCore.Mvc.Models.Abstractions;
using System.Net;

namespace Skywalker.Ddd.AspNetCore.Mvc.Results.Wrapping;

public class BadRequestObjectResultWrapper : IActionResultWrapper
{
    public void Wrap(ResultExecutingContext actionResult)
    {
        if (actionResult.Result is not BadRequestObjectResult badRequestObject)
        {
            throw new ArgumentException($"{nameof(actionResult)} should be BadRequestObjectResult!");
        }

        if (badRequestObject.Value is not ValidationProblemDetails problemDetails)
        {
            throw new ArgumentException($"{nameof(problemDetails)} should be ValidationProblemDetails!");
        }
        var validationErrors = new List<ValidationErrorDto>();
        foreach (var item in problemDetails.Errors)
        {
            foreach (var message in item.Value)
            {
                validationErrors.Add(new ValidationErrorDto(item.Key, message));
            }
        }
        var error = new Error((badRequestObject.StatusCode ?? (int)HttpStatusCode.BadRequest).ToString()!, "")
        {
            ValidationErrors = [.. validationErrors]
        };
        actionResult.Result = new ObjectResult(new AjaxResponse(error));
    }
}
