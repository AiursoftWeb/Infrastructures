using Aiursoft.Pylon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Aiursoft.Pylon.Attributes
{
    /// <summary>
    /// This attribute will not throw any exception but will reject any invalid request directly with AiurCollection with error messages.
    /// Strongly suggest adding this attribute to all API controllers.
    /// </summary>
    public class APIModelStateChecker : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Controller is Controller controller && !controller.ModelState.IsValid)
            {
                context.Result = ResultGenerator.GetInvalidModelStateErrorResponse(controller.ModelState);
            }
            base.OnActionExecuting(context);
        }
    }
    public static class ResultGenerator
    {
        public static JsonResult GetInvalidModelStateErrorResponse(ModelStateDictionary modelstate)
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
