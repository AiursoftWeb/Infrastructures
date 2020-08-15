using Aiursoft.Handler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace Aiursoft.Handler.Services
{
    internal static class ResultGenerator
    {
        internal static JsonResult GetInvalidModelStateErrorResponse(ModelStateDictionary modelState)
        {
            var list = (from value in modelState from error in value.Value.Errors select error.ErrorMessage).ToList();
            var arg = new AiurCollection<string>(list)
            {
                Code = ErrorType.InvalidInput,
                Message = "Your input contains several errors!"
            };
            return new JsonResult(arg);
        }
    }
}
