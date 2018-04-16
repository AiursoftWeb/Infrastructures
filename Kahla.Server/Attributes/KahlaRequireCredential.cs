using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Kahla.Server.Attributes
{
    public class KahlaRequireCredentialAttribute : AiurForceAuth
    {
        //public override void OnActionExecuting(ActionExecutingContext context)
        //{
        //    base.OnActionExecuting(context);
        //    var credential = context.HttpContext.Request.Headers["authorization"];
        //    if (string.IsNullOrEmpty(credential))
        //    {
        //        context.Result = new JsonResult(new AiurProtocal
        //        {
        //            code = ErrorType.Unauthorized,
        //            message = "Unauthorized!"
        //        });
        //    }
        //}
    }
}
