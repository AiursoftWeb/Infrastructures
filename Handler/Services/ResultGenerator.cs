using Aiursoft.Handler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Aiursoft.Handler.Services
{
    internal static class ResultGenerator
    {
        internal static JsonResult GetInvalidModelStateErrorResponse(ModelStateDictionary modelstate)
        {
            var list = new List<string>();
            foreach (var value in modelstate)
            {
                foreach (var error in value.Value.Errors)
                {
                    list.Add(error.ErrorMessage);
                }
            }
            var arg = new AiurCollection<string>(list)
            {
                Code = ErrorType.InvalidInput,
                Message = "Your input contains several errors!"
            };
            return new JsonResult(arg);
        }
    }
}
