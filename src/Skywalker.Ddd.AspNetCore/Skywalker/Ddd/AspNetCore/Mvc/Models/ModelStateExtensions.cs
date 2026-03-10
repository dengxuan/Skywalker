using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Skywalker.Ddd.AspNetCore.Mvc.Models.Abstractions;

namespace Skywalker.Ddd.AspNetCore.Mvc.Models;

public static class ModelStateExtensions
{
    public static AjaxResponse ToMvcAjaxResponse(this ModelStateDictionary modelState)
    {
        if (modelState.IsValid)
        {
            return new AjaxResponse();
        }

        var validationErrors = new List<ValidationError>();

        foreach (var state in modelState)
        {
            foreach (var error in state.Value.Errors)
            {
                if (error.Exception != null)
                {

                }
                validationErrors.Add(new ValidationError(error.ErrorMessage, state.Key));
            }
        }

        var errorInfo = new Error(HttpStatusCode.BadRequest.ToString(), "Bad Request")
        {
            ValidationErrors = [.. validationErrors]
        };

        return new AjaxResponse(errorInfo);
    }
}
