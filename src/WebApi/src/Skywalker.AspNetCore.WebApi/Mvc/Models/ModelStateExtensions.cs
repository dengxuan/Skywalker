﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using Skywalker.AspNetCore.WebApi.Models;
using System.Collections.Generic;

namespace Skywalker.AspNetCore.WebApi.Mvc.Models
{
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

            var errorInfo = new Error()
            {
                ValidationErrors = validationErrors.ToArray()
            };

            return new AjaxResponse(errorInfo);
        }
    }
}
